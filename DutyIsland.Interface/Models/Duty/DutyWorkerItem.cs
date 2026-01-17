using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Duty;

/// <summary>
/// 值日表执行者项
/// </summary>
public partial class DutyWorkerItem : ObservableRecipient
{
    /// <summary>
    /// 执行者姓名
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}