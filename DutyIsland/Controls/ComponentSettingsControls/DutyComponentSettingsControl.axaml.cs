using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Services;
using DutyIsland.Models.ComponentSettings;

namespace DutyIsland.Controls.ComponentSettingsControls;

public partial class DutyComponentSettingsControl : ComponentBase<DutyComponentSettings>
{
    private IDutyPlanService DutyPlanService { get; } = IAppHost.GetService<IDutyPlanService>();
    
    public DutyComponentSettingsControl()
    {
        InitializeComponent();
    }

    private void UseCurrentValueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var templateItem = DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary.GetValueOrDefault(Settings.JobGuid, new DutyPlanTemplateItem
        {
            Name = "???"
        });
        Settings.FallbackSettings.JobName = templateItem?.Name ?? "???";
    }
    
    [RelayCommand]
    private void FallbackWorkersRemoveWorker(DutyWorkerItem item)
    {
        Settings.FallbackSettings.Workers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除项目「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            Settings.FallbackSettings.Workers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }
    
    private void FallbackWorkersAddWorkerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Settings.FallbackSettings.Workers.Add(new DutyWorkerItem());
    }
}