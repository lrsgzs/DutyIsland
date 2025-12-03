using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Models.ActionSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Services;
using FluentAvalonia.UI.Controls;

namespace DutyIsland.Controls.ActionSettingsControls;

public partial class NotifyDutyActionSettingsControl : ActionSettingsControlBase<NotifyDutyActionSettings>
{
    private DutyPlanService DutyPlanService { get; } = IAppHost.GetService<DutyPlanService>();
    
    public NotifyDutyActionSettingsControl()
    {
        InitializeComponent();
    }

    private void ButtonShowSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.FindResource("NotificationSettingsDrawer") is not ContentControl cc) return;
        cc.DataContext = this;
        _ = ShowDrawer(cc);
    }
    
    private void UseCurrentValueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var templateItem = DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary.GetValueOrDefault(Settings.JobGuid, new DutyPlanTemplateItem
        {
            Name = "???"
        });
        Settings.FallbackJobName = templateItem?.Name ?? "???";
    }
    
    [RelayCommand]
    private void FallbackWorkersRemoveWorker(WorkerItem item)
    {
        Settings.FallbackWorkers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除项目「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            Settings.FallbackWorkers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }
    
    private void FallbackWorkersAddWorkerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Settings.FallbackWorkers.Add(new WorkerItem());
    }
}