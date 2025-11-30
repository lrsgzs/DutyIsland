using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private Profile.Profile _profile = new();
    
    [ObservableProperty] private bool _globalEnableNotification = true;
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