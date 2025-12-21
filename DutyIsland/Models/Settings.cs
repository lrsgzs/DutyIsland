using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Enums;

namespace DutyIsland.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private Profile.Profile _profile = new();
    
    [ObservableProperty] private DutyPlanGetMode _dutyPlanGetMode = DutyPlanGetMode.AttachedSettings;
    [ObservableProperty] private bool _globalEnableNotification = true;
    [ObservableProperty] private TimeSource _timeSource = TimeSource.ClassIsland;
    private bool _enableSentry = true;

    public bool EnableSentry
    {
        get => _enableSentry;
        set
        {
            _enableSentry = value;
            OnPropertyChanged();
            RestartPropertyChanged?.Invoke();
        }
    }

    public event Action? RestartPropertyChanged;
}