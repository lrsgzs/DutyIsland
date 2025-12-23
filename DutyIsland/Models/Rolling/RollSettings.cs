using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Rolling;

public partial class RollSettings : ObservableRecipient
{
    [ObservableProperty] private ObservableCollection<RollItem> _rollItems = [];
    [ObservableProperty] private int _rollIndex = 0;
    
    [ObservableProperty] private DateOnly _lastChangedDate;
    [ObservableProperty] private int _rollDays = 1;
    [ObservableProperty] private bool _rollOnUnopenDay = false;
    [ObservableProperty] private bool _skipWeekend = true;
}