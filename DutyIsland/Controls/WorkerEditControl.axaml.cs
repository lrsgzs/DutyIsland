using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;

namespace DutyIsland.Controls;

public partial class WorkerEditControl : UserControl
{
    public static readonly StyledProperty<string> WorkerProperty =
        AvaloniaProperty.Register<WorkerEditControl, string>(
            nameof(Worker), defaultBindingMode: BindingMode.TwoWay);
    
    public string Worker
    {
        get => GetValue(WorkerProperty);
        set => SetValue(WorkerProperty, value);
    }

    private ObservableCollection<WorkerItem> WorkerItems { get; } = GlobalConstants.Config!.Data.Profile.Workers;
    public AutoCompleteFilterPredicate<object> FilterWorker { get; } = _FilterWorker;

    public WorkerEditControl()
    {
        InitializeComponent();
    }

    private static bool _FilterWorker(string? search, object item)
    {
        if (item as dynamic is not WorkerItem worker)
        {
            return false;
        }
        
        return worker.Name.Contains(search ?? "", StringComparison.CurrentCultureIgnoreCase)
               || worker.Id.Contains(search ?? "", StringComparison.CurrentCultureIgnoreCase)
               || PinyinHelper.GetFullPinyinList(worker.Name)
                   .Any(pinyin => pinyin.StartsWith(search ?? "", StringComparison.CurrentCultureIgnoreCase))
               || PinyinHelper.GetFirstPinyinList(worker.Name)
                   .Any(pinyin => pinyin.StartsWith(search ?? "", StringComparison.CurrentCultureIgnoreCase));
    }
}