using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Models.Duty;

namespace DutyIsland.Controls;

public partial class WorkersDataGridControl : UserControl
{
    public static readonly StyledProperty<ObservableCollection<DutyWorkerItem>> WorkersProperty =
        AvaloniaProperty.Register<WorkersDataGridControl, ObservableCollection<DutyWorkerItem>>(
            nameof(Workers), defaultBindingMode: BindingMode.TwoWay);
    
    public ObservableCollection<DutyWorkerItem> Workers
    {
        get => GetValue(WorkersProperty);
        set => SetValue(WorkersProperty, value);
    }
    
    public WorkersDataGridControl()
    {
        InitializeComponent();
    }
    
    [RelayCommand]
    private void FallbackWorkersRemoveWorker(DutyWorkerItem item)
    {
        Workers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除项目「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            Workers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }
    
    private void FallbackWorkersAddWorkerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Workers.Add(new DutyWorkerItem());
    }
}