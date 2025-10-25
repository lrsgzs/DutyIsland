using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
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
}