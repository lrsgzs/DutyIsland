using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;

namespace DutyIsland.Views.SettingPages;

/// <summary>
/// 「SuperAutoIsland 主设置」视图
/// </summary>
[FullWidthPage]
[HidePageTitle]
[SettingsPageInfo("duty.settings.duty","DutyIsland 值日表","\uE31E","\uE31D")]
public partial class DutySettingsPage : SettingsPageBase {
    public DutySettingsPage()
    {
        InitializeComponent();
    }
}