using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class WorkerItem : ObservableRecipient
{
    [ObservableProperty] private string _name = string.Empty;
}