using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using DutyIsland.Shared;

namespace DutyIsland.Services.NotificationProviders;

[NotificationProviderInfo(GlobalConstants.DutyNotificationProviderGuid, "值日人员提醒", "\uE31E", "提醒值日人员打扫。")]
[NotificationChannelInfo(GlobalConstants.DutyActionNotificationChannelGuid, "值日行动提醒", "\uE314", description:"通过行动提醒。")]
public class DutyNotificationProvider : NotificationProviderBase
{
    public async Task StartAsync(CancellationToken cancellationToken) { }
    public async Task StopAsync(CancellationToken cancellationToken) { }
}