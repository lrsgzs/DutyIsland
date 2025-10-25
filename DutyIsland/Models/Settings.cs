using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private Profile.Profile _profile = new();
    
    [ObservableProperty] private bool _globalEnableNotification = true;
}