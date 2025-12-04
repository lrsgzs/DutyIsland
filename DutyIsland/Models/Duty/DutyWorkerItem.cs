using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyWorkerItem : ObservableRecipient
{
    [ObservableProperty] private string _name = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}