using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using ClassIsland.Core.Helpers.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Enums;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;
using DynamicData;

namespace DutyIsland.Controls;

public partial class WorkerEditControl : UserControl
{
    public partial class WorkerEditControlModel : ObservableRecipient
    {
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private bool _isFilteredMale = true;
        [ObservableProperty] private bool _isFilteredFemale = true;
        [ObservableProperty] private bool _isFilteredUnknown = true;
        
        [ObservableProperty] private ObservableCollection<WorkerItem> _workerItems = [];
        [ObservableProperty] private WorkerItem? _selectedWorkerItem = null;

    }
    
    public static readonly StyledProperty<string> WorkerProperty =
        AvaloniaProperty.Register<WorkerEditControl, string>(
            nameof(Worker), defaultBindingMode: BindingMode.TwoWay);
    
    public string Worker
    {
        get => GetValue(WorkerProperty);
        set => SetValue(WorkerProperty, value);
    }

    public WorkerEditControlModel Model { get; } = new();

    public WorkerEditControl()
    {
        Model.WorkerItems.AddRange(GlobalConstants.Config!.Data.Profile.Workers);
        InitializeComponent();
    }

    private void TextBoxSearch_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }
        
        SearchWorker();
    }

    private void ButtonSearch_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchWorker();
    }
    
    private void SearchWorker()
    {
        Model.WorkerItems.Clear();
        Model.WorkerItems.AddRange(
            GlobalConstants.Config!.Data.Profile.Workers
                .Where(item =>
                {
                    var isFiltered = item.Sex switch
                    {
                        HumanSex.Male => Model.IsFilteredMale,
                        HumanSex.Female => Model.IsFilteredFemale,
                        _ => Model.IsFilteredUnknown
                    };

                    if (string.IsNullOrWhiteSpace(Model.SearchText))
                    {
                        return isFiltered;
                    }
                    return isFiltered && (item.Name.Contains(Model.SearchText) || item.Id.Contains(Model.SearchText));
                }));
    }

    private void ButtonApply_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Model.SelectedWorkerItem == null)
        {
            return;
        }

        Worker = Model.SelectedWorkerItem.Name;
        FlyoutHelper.CloseAncestorFlyout(sender);
    }
}