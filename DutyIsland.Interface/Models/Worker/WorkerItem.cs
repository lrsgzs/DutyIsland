using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Enums;

namespace DutyIsland.Interface.Models.Worker;

/// <summary>
/// 执行者项
/// </summary>
public partial class WorkerItem : ObservableRecipient
{
    /// <summary>
    /// 执行者姓名
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;
    
    /// <summary>
    /// 执行者编号
    /// </summary>
    [ObservableProperty] private string _id = string.Empty;
    
    /// <summary>
    /// 执行者性别
    /// </summary>
    [ObservableProperty] private HumanSex _sex = HumanSex.Unknown;

    public override string ToString()
    {
        return Name;
    }
}