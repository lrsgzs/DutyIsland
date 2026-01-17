using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Rolling;

/// <summary>
/// 轮换设置
/// </summary>
public partial class RollSettings : ObservableRecipient
{
    /// <summary>
    /// 轮换项
    /// </summary>
    [ObservableProperty] private ObservableCollection<RollItem> _rollItems = [];
    
    /// <summary>
    /// 当前轮换索引
    /// </summary>
    [ObservableProperty] private int _rollIndex = 0;
    
    /// <summary>
    /// 上次轮换日期
    /// </summary>
    [ObservableProperty] private DateOnly _lastChangedDate;
    
    /// <summary>
    /// 轮换间隔天数
    /// </summary>
    [ObservableProperty] private int _rollDays = 1;
    
    /// <summary>
    /// 是否未开启 ClassIsland 也轮回
    /// </summary>
    [ObservableProperty] private bool _rollOnUnopenDay = false;
    
    /// <summary>
    /// 是否跳过周末
    /// </summary>
    [ObservableProperty] private bool _skipWeekend = true;
}