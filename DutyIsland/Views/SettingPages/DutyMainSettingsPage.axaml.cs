using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using DutyIsland.Shared;
using DutyIsland.ViewModels.SettingPages;
using FluentAvalonia.UI.Controls;

namespace DutyIsland.Views.SettingPages;

[FullWidthPage]
[HidePageTitle]
[Group("duty.settings")]
[SettingsPageInfo("duty.settings.main","主设置","\uE994","\uE993")]
public partial class DutyMainSettingsPage : SettingsPageBase
{
    private DutyViewModel ViewModel { get; } = IAppHost.GetService<DutyViewModel>();
    private string PluginVersion { get; } = GlobalConstants.Information.PluginVersion;
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
        var license = await File.ReadAllTextAsync(Path.Combine(GlobalConstants.Information.PluginFolder, "LICENSE"));
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