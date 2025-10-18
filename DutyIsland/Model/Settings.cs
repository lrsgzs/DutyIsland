using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Model;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private Profile.Profile _profile = new();
}