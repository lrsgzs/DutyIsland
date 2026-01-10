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
using DutyIsland.Models.Rolling;
using DutyIsland.Models.Worker;
using DutyIsland.Services;
using DutyIsland.Shared;
using DutyIsland.ViewModels.SettingPages;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace DutyIsland.Views.SettingPages;

[FullWidthPage]
[HidePageTitle]
[Group("duty.settings")]
[SettingsPageInfo("duty.settings.main","主设置","\uE994","\uE993")]
public partial class DutyMainSettingsPage : SettingsPageBase
{
    private DutyPlanService DutyPlanService { get; } = IAppHost.GetService<DutyPlanService>();
    private DutyViewModel ViewModel { get; } = IAppHost.GetService<DutyViewModel>();
    private ImportWorkersWindow? ImportWorkersWindow { get; set; }
    private string PluginVersion { get; } = GlobalConstants.PluginVersion;
    private bool _notifiedRestart = false;
    
    public DutyMainSettingsPage()
    {
        DataContext = this;
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

    #region Settings

    private async void SettingsExpanderItemShowOssLicense_OnClick(object? sender, RoutedEventArgs e)
    {
        var license = await File.ReadAllTextAsync(GlobalConstants.PluginFolder + "/LICENSE");
        await new ContentDialog()
        {
            Title = "开放源代码许可",
            Content = new TextBox
            {
                Text = license,
                IsReadOnly = true
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