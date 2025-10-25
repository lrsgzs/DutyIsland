using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using DutyIsland.Models.Duty;
using DutyIsland.ViewModels.SettingPages;

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

    #endregion
}