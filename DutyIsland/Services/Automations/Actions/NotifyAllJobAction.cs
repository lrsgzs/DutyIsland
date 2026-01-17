using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using DutyIsland.Extensions;
using DutyIsland.Interface.Models.Notification;
using DutyIsland.Interface.Services;
using DutyIsland.Services.NotificationProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland.Services.Automations.Actions;

[ActionInfo("duty.actions.notifyAllJob", "提醒所有值日人员", "\uE314")]
public class NotifyAllJobAction : ActionBase
{
    private IDutyPlanService DutyPlanService { get; }
    private DutyNotificationProvider DutyNotificationProvider { get; }
    private List<NotificationRequest>? _requests;
    private bool _isStopped = false;
    
    public NotifyAllJobAction(IDutyPlanService dutyPlanService)
    {
        DutyPlanService = dutyPlanService;
        DutyNotificationProvider = IAppHost.Host!.Services.GetServices<IHostedService>().OfType<DutyNotificationProvider>().First();
    }
    
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();

        List<AutoNotificationInfo> infos = [];
        infos.AddRange(
            (DutyPlanService.CurrentDutyPlan?.ComplexItems?.List ?? [])
            .Select(kvp =>
                new AutoNotificationInfo
                {
                    Guid = kvp.Key,
                    Item = kvp.Value.First,
                    TemplateItem = kvp.Value.Second,
                    NotificationSettings = kvp.Value.Second.NotificationSettings
                }));
        _requests = infos.ToRequests();
        
        ActionItem.Progress = 0;
        for (var i = 0; i < _requests.Count; i++)
        {
            await DutyNotificationProvider.ShowActionNotification(_requests[i]);
            await Task.Delay(1);
            ActionItem.Progress = (i + 1) * 1.0 / _requests.Count * 100;

            if (_isStopped)
            {
                break;
            }
        }

        ActionItem.Progress = 100;
        _isStopped = false;
    }
    
    protected override async Task OnInterrupted()
    {
        await base.OnInterrupted();
        
        _ = Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var request in _requests ?? [])
            {
                request.Cancel();
            }
        });
        _isStopped = true;
    }
}