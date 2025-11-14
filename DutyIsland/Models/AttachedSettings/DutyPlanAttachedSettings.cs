using ClassIsland.Shared.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.AttachedSettings;

public partial class DutyPlanAttachedSettings : ObservableRecipient, IAttachedSettings
{
    [ObservableProperty] private bool _isAttachSettingsEnabled = false;
    [ObservableProperty] private Guid? _dutyPlanGuid = null;
}