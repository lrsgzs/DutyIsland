using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using DutyIsland.Models.Duty;
using DutyIsland.ViewModels.SettingPages;
using DynamicData;
using ReactiveUI;

namespace DutyIsland.Views.SettingPages;

[FullWidthPage]
[HidePageTitle]
[SettingsPageInfo("duty.settings.duty","DutyIsland 值日表","\uE31E","\uE31D")]
public partial class DutySettingsPage : SettingsPageBase
{
    private DutyViewModel ViewModel { get; } = IAppHost.GetService<DutyViewModel>();
    
    public DutySettingsPage()
    {
        InitializeComponent();
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

    #endregion
}