using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Enums;

namespace DutyIsland.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private Profile.Profile _profile = new();
    
    [ObservableProperty] private DutyPlanGetMode _dutyPlanGetMode = DutyPlanGetMode.AttachedSettings;
    [ObservableProperty] private bool _enableTaskBarNotificationMenu = true;
    [ObservableProperty] private bool _globalEnableNotification = true;
    [ObservableProperty] private TimeSource _timeSource = TimeSource.ClassIsland;
    private bool _enableSentry = true;
    private bool _enableExperimentFeature = false;
    
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

    public bool EnableExperimentFeature
    {
        get => _enableExperimentFeature;
        set
        {
            _enableExperimentFeature = value;
            OnPropertyChanged();
            RestartPropertyChanged?.Invoke();
        }
    }

    public event Action? RestartPropertyChanged;
}