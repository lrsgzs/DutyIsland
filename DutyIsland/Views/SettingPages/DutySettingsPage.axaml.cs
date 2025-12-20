using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Enums;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Notification;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;
using DutyIsland.ViewModels.SettingPages;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace DutyIsland.Views.SettingPages;

[FullWidthPage]
[HidePageTitle]
[SettingsPageInfo("duty.settings.duty","DutyIsland 值日表","\uE31E","\uE31D")]
public partial class DutySettingsPage : SettingsPageBase
{
    private DutyViewModel ViewModel { get; } = IAppHost.GetService<DutyViewModel>();
    private ILessonsService LessonsService { get; } = IAppHost.GetService<ILessonsService>();
    private ImportWorkersWindow? ImportWorkersWindow { get; set; }
    private string PluginVersion { get; } = GlobalConstants.PluginVersion;
    private bool _notifiedRestart = false;
    
    public DutySettingsPage()
    {
        InitializeComponent();
        ViewModel.Settings.RestartPropertyChanged += () =>
        {
            if (_notifiedRestart) return;
            
            RequestRestart();
            _notifiedRestart = true;
        };
    }

    #region Misc

    private void ButtonSave_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.ConfigHandler.Save();
        this.ShowToast(new ToastMessage
        {
            Message = "保存成功",
            Duration = TimeSpan.FromSeconds(5)
        });
    }

    #endregion

    #region DutyPlan
    
    private void ButtonAddDutyPlan_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateDutyPlan();
    }

    private void ButtonDuplicateDutyPlan_OnClick(object? sender, RoutedEventArgs e)
    {
        var s = ConfigureFileHelper.CopyObject(ViewModel.SelectedDutyPlan);
        if (s == null) return;
        
        ViewModel.Settings.Profile.DutyPlans.Add(Guid.NewGuid(), s);
        ViewModel.SelectedDutyPlan = s;
        UpdateDutyPlanSelectedDutyPlanTemplateKvp();
    }

    private void CreateDutyPlan()
    {
        var newDutyPlan = new DutyPlan();
        ViewModel.Settings.Profile.DutyPlans.Add(Guid.NewGuid(), newDutyPlan);
        ViewModel.SelectedDutyPlan = newDutyPlan;
        UpdateDutyPlanSelectedDutyPlanTemplateKvp();
    }

    private void UpdateDutyPlanSelectedDutyPlanTemplateKvp()
    {
        if (ViewModel.SelectedDutyPlan?.TemplateGuid == null)
        {
            ViewModel.DutyPlanSelectedDutyPlanTemplateKvp = null;
        }
        else
        {
            var kvp = ViewModel.DutyPlanTemplates.List.FirstOrDefault(x => x.Key == ViewModel.SelectedDutyPlan.TemplateGuid);
            ViewModel.DutyPlanSelectedDutyPlanTemplateKvp = kvp;
        }
    }

    private void ButtonDeleteSelectedDutyPlan_OnClick(object? sender, RoutedEventArgs e)
    {
        var key = ViewModel.Settings.Profile.DutyPlans
            .FirstOrDefault(x => x.Value == ViewModel.SelectedDutyPlan).Key;

        ViewModel.Settings.Profile.DutyPlans.Remove(key);
        FlyoutHelper.CloseAncestorFlyout(sender);
    }

    private void ButtonImportDutyPlan_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedDutyPlan == null) return;

        if (ViewModel.SelectedDutyPlan.Template == null)
        {
            this.ShowToast(new ToastMessage("注意到值日表模板为空，无法导入喵")
            {
                Duration = TimeSpan.FromSeconds(10),
                Severity = InfoBarSeverity.Warning
            });
            return;
        }
        
        if (string.IsNullOrWhiteSpace(ViewModel.ImportDutyPlanText))
        {
            this.ShowToast(new ToastMessage("注意到导入文本为空，不建议哦")
            {
                Duration = TimeSpan.FromSeconds(10),
                Severity = InfoBarSeverity.Warning
            });
            return;
        }

        var peopleList = ViewModel.ImportDutyPlanText
            .Split("\n")
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .ToList();

        if (peopleList.Count == 0)
        {
            this.ShowToast(new ToastMessage("注意到导入文本没有有效人员")
            {
                Duration = TimeSpan.FromSeconds(10),
                Severity = InfoBarSeverity.Warning
            });
            return;
        }
        
        var templateItems = ViewModel.SelectedDutyPlan.ComplexItems;
        var workerDictionary = ViewModel.SelectedDutyPlan.WorkerDictionary;

        var currentPeopleIndex = 0;
        for (var i = 0; i < templateItems!.List.Count && currentPeopleIndex < peopleList.Count; i++)
        {
            var templateKvp = templateItems.List[i];
            ObservableCollection<DutyWorkerItem> workers = [];
            
            for (var j = 0; j < templateKvp.Value.Second.WorkerCount && currentPeopleIndex < peopleList.Count; j++)
            {
                workers.Add(new DutyWorkerItem { Name = peopleList[currentPeopleIndex] });
                currentPeopleIndex++;
            }

            if (workerDictionary.TryGetValue(templateKvp.Key, out var item))
            {
                item.Workers = workers;
            }
            else
            {
                workerDictionary[templateKvp.Key] = new DutyPlanItem { Workers = workers };
            }
        }

        ViewModel.ImportDutyPlanText = "";
        FlyoutHelper.CloseAncestorFlyout(sender);
    }
    
    private void DutyPlanAddWorkerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedDutyPlanItem?.Workers.Add(new DutyWorkerItem());
    }
    
    [RelayCommand]
    private void DutyPlanRemoveWorker(DutyWorkerItem item)
    {
        ViewModel.SelectedDutyPlanItem?.Workers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除项目「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            ViewModel.SelectedDutyPlanItem?.Workers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }

    #endregion

    #region DutyPlanTemplate
    
    private void ButtonAddDutyPlanTemplate_OnClick(object? sender, RoutedEventArgs e)
    {
        var newDutyPlanTemplate = new DutyPlanTemplate();
        ViewModel.Settings.Profile.DutyPlanTemplates.Add(Guid.NewGuid(), newDutyPlanTemplate);
        ViewModel.SelectedDutyPlanTemplate = newDutyPlanTemplate;
    }
    
    private void ButtonDuplicateDutyPlanTemplate_OnClick(object? sender, RoutedEventArgs e)
    {
        var s = ConfigureFileHelper.CopyObject(ViewModel.SelectedDutyPlanTemplate);
        if (s == null) return;
        
        ViewModel.Settings.Profile.DutyPlanTemplates.Add(Guid.NewGuid(), s);
        ViewModel.SelectedDutyPlanTemplate = s;
        UpdateDutyPlanSelectedDutyPlanTemplateKvp();
    }

    private void ButtonDeleteSelectedDutyPlanTemplate_OnClick(object? sender, RoutedEventArgs e)
    {
        var key = ViewModel.Settings.Profile.DutyPlanTemplates
            .FirstOrDefault(x => x.Value == ViewModel.SelectedDutyPlanTemplate).Key;
        
        var c = ViewModel.Settings.Profile.DutyPlans.Any(x => x.Value.TemplateGuid == key);
        if (c)
        {
            this.ShowWarningToast("仍有值日表在使用该模板。删除值日表模型前需要删除所有使用该模板的值日表。");
            return;
        }

        ViewModel.Settings.Profile.DutyPlanTemplates.Remove(key);
        FlyoutHelper.CloseAncestorFlyout(sender);
    }

    private void ButtonAddDutyPlanTemplateItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var newItem = new DutyPlanTemplateItem();
        var itemGuid = Guid.NewGuid();
        
        ViewModel.SelectedDutyPlanTemplate!.WorkerTemplateDictionary.Add(itemGuid, newItem);
        ViewModel.SelectedDutyPlanTemplateItem = itemGuid;
    }

    private void ButtonRemoveDutyPlanTemplateItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedDutyPlanTemplateItem == null) 
            return;
        
        var templateItemGuid = ViewModel.SelectedDutyPlanTemplateItem.Value;
        var dutyPlanTemplate = ViewModel.SelectedDutyPlanTemplate;
        
        if (dutyPlanTemplate == null)
        {
            return;
        }

        var templateItem = dutyPlanTemplate.WorkerTemplateDictionary[templateItemGuid];
        dutyPlanTemplate.WorkerTemplateDictionary.Remove(templateItemGuid);
        ViewModel.SelectedDutyPlanTemplateItemKvp = null;
        
        var revertButton = new Button()
        {
            Content = "撤销"
        };

        ViewModel.CurrentTemplateItemDeleteRevertToast?.Close();
        var message = ViewModel.CurrentTemplateItemDeleteRevertToast = new ToastMessage()
        {
            Message = $"已删除任务 {templateItem}。",
            Duration = TimeSpan.FromSeconds(10),
            ActionContent = revertButton
        };
        
        revertButton.Click += RevertButtonOnClick;
        // message.ClosedCancellationTokenSource.Token.Register(() =>
        // {
        //     revertButton.Click -= RevertButtonOnClick;
        //     ViewModel.CurrentTemplateItemDeleteRevertToast = null;
        // });
        
        ViewModel.ObservableForProperty(x => x.SelectedDutyPlanTemplate).Subscribe(_ => message.Close());
        this.ShowToast(message);
        
        return;

        void RevertButtonOnClick(object? o, RoutedEventArgs routedEventArgs)
        {
            ViewModel.SelectedDutyPlanTemplate!.WorkerTemplateDictionary.Add(templateItemGuid, templateItem);
            ViewModel.SelectedDutyPlanTemplateItem = templateItemGuid;
            message.Close();
        }
    }
    
    private void DutyPlanTemplateAddNotificationTimeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedDutyPlanTemplateItemKvp?.Value.NotificationTimes.Times.Add(new NotificationTimeItem());
    }
    
    [RelayCommand]
    private void DutyPlanTemplateRemoveNotificationTime(NotificationTimeItem item)
    {
        ViewModel.SelectedDutyPlanTemplateItemKvp?.Value.NotificationTimes.Times.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除时间「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            ViewModel.SelectedDutyPlanTemplateItemKvp?.Value.NotificationTimes.Times.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }

    #endregion

    #region Worker
    
    private void ButtonAddWorker_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Workers.Add(new WorkerItem());
    }

    private void ButtonRemoveWorker_OnClick(object? sender, RoutedEventArgs e)
    {
        var item = ViewModel.SelectedWorkerItem;
        if (item == null)
        {
            return;
        }
        
        ViewModel.Workers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除人员「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            ViewModel.Workers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }

    private void ButtonImportWorkers_OnClick(object? sender, RoutedEventArgs e)
    {
        ImportWorkersWindow = new ImportWorkersWindow();
        ImportWorkersWindow.Show();
    }

    [RelayCommand]
    private void WorkerItemSetSex(HumanSex sex)
    {
        if (ViewModel.SelectedWorkerItem != null)
        {
            ViewModel.SelectedWorkerItem.Sex = sex;
        }
    }
    
    #endregion

    #region Settings

    private async void SettingsExpanderItemShowOssLicense_OnClick(object? sender, RoutedEventArgs e)
    {
        var license = await File.ReadAllTextAsync(GlobalConstants.PluginFolder + "/LICENSE");
        await new ContentDialog()
        {
            Title = "开放源代码许可",
            Content = new TextBlock()
            {
                Text = license
            },
            PrimaryButtonText = "关闭",
            DefaultButton = ContentDialogButton.Primary
        }.ShowAsync();
    }
    
    private void UIElementAppInfo_OnMouseDown(object? sender, RoutedEventArgs pointerPressedEventArgs)
    {
        ViewModel.AppIconClickCount++;
        if (ViewModel.AppIconClickCount >= 20)
        {
            TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync("5rS+6JKZ77yM5pyA5aW955qE5LyZ5Ly077yB");
        }
    }

    private void UriNavigationCommands_OnClick(object sender, RoutedEventArgs e)
    {
        var url = e.Source switch
        {
            SettingsExpanderItem s => s.CommandParameter?.ToString(),
            Button s => s.CommandParameter?.ToString(),
            _ => "classisland://app/test/"
        };
        IAppHost.TryGetService<IUriNavigationService>()?.NavigateWrapped(new Uri(url!));
    }

    #endregion
}