using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Enums;

namespace DutyIsland.Models.Worker;

public partial class WorkerItem : ObservableRecipient
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _id = string.Empty;
    [ObservableProperty] private HumanSex _sex = HumanSex.Unknown;

    public override string ToString()
    {
        return Name;
    }
}