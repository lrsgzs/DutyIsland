using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Worker;

namespace DutyIsland.Controls;

public partial class FallbackWorkersDataGridControl : UserControl
{
    public static readonly DirectProperty<FallbackWorkersDataGridControl, FallbackSettings> SettingsProperty =
        AvaloniaProperty.RegisterDirect<FallbackWorkersDataGridControl, FallbackSettings>(
            nameof(Settings), o => o.Settings, (o, v) => o.Settings = v);

    private FallbackSettings _settings = new();
    
    public FallbackSettings Settings
    {
        get => _settings;
        set => SetAndRaise(SettingsProperty, ref _settings, value);
    }
    
    public FallbackWorkersDataGridControl()
    {
        DataContext = this;
        InitializeComponent();
    }
    
    [RelayCommand]
    private void FallbackWorkersRemoveWorker(DutyWorkerItem item)
    {
        Settings.Workers.Remove(item);
        
        var revertButton = new Button { Content = "撤销" };
        var toastMessage = new ToastMessage($"已删除项目「{item}」。")
        {
            ActionContent = revertButton,
            Duration = TimeSpan.FromSeconds(10)
        };
        
        revertButton.Click += (o, args) =>
        {
            Settings.Workers.Add(item);
            toastMessage.Close();
        };
        
        this.ShowToast(toastMessage);
    }
    
    private void FallbackWorkersAddWorkerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Settings.Workers.Add(new DutyWorkerItem());
    }
}