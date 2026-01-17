using dotnetCampus.Ipc.CompilerServices.Attributes;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Models.Profile;

namespace DutyIsland.Interface.Services;

/// <summary>
/// 公开的值日表服务，用于 Ipc。
/// </summary>
[IpcPublic(IgnoresIpcException = true)]
public interface IPublicDutyPlanService
{
    /// <summary>
    /// 当前的值日表
    /// </summary>
    public DutyPlan? CurrentDutyPlan { get; }
    
    /// <summary>
    /// 档案
    /// </summary>
    public Profile Profile { get; }
    
    /// <summary>
    /// 刷新值日表
    /// </summary>
    public void RefreshDutyPlan();
}