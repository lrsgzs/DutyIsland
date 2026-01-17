using ClassIsland.Shared;
using ClassIsland.Shared.IPC;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Models.Notification;
using DutyIsland.Interface.Models.Worker;

namespace DutyIsland.Interface.Services;

/// <summary>
/// 值日表服务，可以拉取各种信息。
/// </summary>
public interface IDutyPlanService : IPublicDutyPlanService
{
    /// <summary>
    /// 值日表刷新事件，每 4 ticks 触发一次，约 200ms
    /// </summary>
    public event EventHandler? WhenRefreshDutyPlan;
    
    /// <summary>
    /// 值日表变化事件
    /// </summary>
    public event EventHandler? WhenDutyPlanChanged;
    
    /// <summary>
    /// 自动提醒事件，参数为 <see cref="AutoNotificationInfo"/>
    /// </summary>
    public event EventHandler<AutoNotificationInfo>? OnDutyJobAutoNotificationEvent;

    private static IpcClient? _ipcClient;
    private static IPublicDutyPlanService? _service;
    
    /// <summary>
    /// 更新值日表轮换索引
    /// </summary>
    /// <param name="currentDate">当前日期</param>
    public void UpdateRollingIndex(DateOnly currentDate);
    
    /// <summary>
    /// 通过附加设置获取值日表 Guid
    /// </summary>
    /// <returns>值日表的 Guid</returns>
    public Guid? GetDutyPlanGuidByAttachedSettings();
    
    /// <summary>
    /// 通过轮换设置获取值日表 Guid
    /// </summary>
    /// <returns>值日表的 Guid</returns>
    public Guid? GetDutyPlanGuidByRollingSettings();
    
    /// <summary>
    /// 检查值日自动提醒时间
    /// </summary>
    public void CheckDutyJobNotificationTime();
    
    /// <summary>
    /// 获取值日表模板项
    /// </summary>
    /// <param name="jobGuid">任务的 Guid</param>
    /// <param name="fallbackSettings">回滚设置</param>
    /// <returns>获取到的值日表模板项</returns>
    public DutyPlanTemplateItem? GetTemplateItem(Guid jobGuid, FallbackSettings fallbackSettings);
    
    /// <summary>
    /// 获取执行者文本内容
    /// </summary>
    /// <param name="jobGuid">任务的 Guid</param>
    /// <param name="fallbackSettings">回滚设置</param>
    /// <param name="connectorString">连接符</param>
    /// <returns>拼接得到的文本内容</returns>
    public string GetWorkersContent(Guid jobGuid, FallbackSettings fallbackSettings, string connectorString = "、");
    
    /// <summary>
    /// 执行者转字符串
    /// </summary>
    /// <param name="workers">值日表执行者列表</param>
    /// <param name="connectorString">连接符</param>
    /// <returns>拼接得到的字符串</returns>
    public static string WorkersToString(IEnumerable<DutyWorkerItem> workers, string connectorString)
    {
        var text = string.Empty;
        foreach (var workerItem in workers)
        {
            if (text == string.Empty)
            {
                text += workerItem.Name;
            }
            else
            {
                text += $"{connectorString}{workerItem.Name}";
            }
        }

        return text;
    }

    /// <summary>
    /// 渲染字符串，用于展示值日信息
    /// </summary>
    /// <param name="text">原文本</param>
    /// <param name="workersText">执行者文本</param>
    /// <param name="dutyPlanTemplateItem">值日表模板项</param>
    /// <returns>渲染得到的字符串</returns>
    public static string FormatString(string text, string workersText, DutyPlanTemplateItem? dutyPlanTemplateItem)
    {
        return text
            .Replace("%j", dutyPlanTemplateItem?.Name ?? "???")
            .Replace("%n", workersText);
    }
    
    /// <summary>
    /// 获取服务
    /// </summary>
    /// <returns>获取到的 IPublicDutyPlanService 服务</returns>
    /// <seealso cref="IPublicDutyPlanService"/>
    public static IPublicDutyPlanService GetService()
    {
        if (_service != null)
        {
            return _service;
        }
        
        var hostService = IAppHost.TryGetService<IDutyPlanService>();
        if (hostService != null)
        {
            return _service = hostService;
        }

        if (_ipcClient == null)
        {
            _ipcClient = new IpcClient();
            _ipcClient.Connect().Wait();
        }

        return _service = _ipcClient.Provider.CreateIpcProxy<IPublicDutyPlanService>(_ipcClient.PeerProxy!);
    }
}