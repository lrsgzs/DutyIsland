using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Rolling;
using DutyIsland.Interface.Services;
using DutyIsland.ViewModels.SettingPages;

namespace DutyIsland.Views.SettingPages;

[FullWidthPage]
[HidePageTitle]
[Group("duty.settings")]
[SettingsPageInfo("duty.settings.rolling","轮换","\uE356","\uE355")]
public partial class DutyRollingSettingsPage : SettingsPageBase
{
    private IDutyPlanService DutyPlanService { get; } = IAppHost.GetService<IDutyPlanService>();
    private DutyViewModel ViewModel { get; } = IAppHost.GetService<DutyViewModel>();
    
    public DutyRollingSettingsPage()
    {
        DataContext = this;
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

    #region Rolling
    
    private void ButtonAddRollItem_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Rolling.RollItems.Add(new RollItem());
    }

    private void ButtonRemoveRollItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedRollItem == null) 
            return;

        var item = ViewModel.SelectedRollItem;
        ViewModel.Rolling.RollItems.Remove(ViewModel.SelectedRollItem);
        ViewModel.SelectedRollItem = null;
        
        var revertButton = new Button
        {
            Content = "撤销"
        };

        ViewModel.CurrentTemplateItemDeleteRevertToast?.Close();
        var message = ViewModel.CurrentTemplateItemDeleteRevertToast = new ToastMessage
        {
            Message = $"已删除项目 {item}。",
            Duration = TimeSpan.FromSeconds(10),
            ActionContent = revertButton
        };
        
        revertButton.Click += RevertButtonOnClick;
        this.ShowToast(message);
        
        return;

        void RevertButtonOnClick(object? o, RoutedEventArgs routedEventArgs)
        {
            ViewModel.Rolling.RollItems.Add(item);
            message.Close();
        }
    }

    private void ButtonManualUpdateRolling_OnClick(object? sender, RoutedEventArgs e)
    {
        DutyPlanService.UpdateRollingIndex(DateOnly.FromDateTime(DateTime.Now));;
        this.ShowToast(new ToastMessage
        {
            Message = "已成功刷新轮换状态。",
            Duration = TimeSpan.FromSeconds(5)
        });
    }

    #endregion
}