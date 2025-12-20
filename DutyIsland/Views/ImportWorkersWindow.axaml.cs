using Avalonia.Interactivity;
using ClassIsland.Core.Controls;
using ClassIsland.Shared;
using DutyIsland.Shared;
using DutyIsland.ViewModels;
using DynamicData;

namespace DutyIsland.Views;

public partial class ImportWorkersWindow : MyWindow
{
    public ImportWorkersViewModel ViewModel { get; } = IAppHost.GetService<ImportWorkersViewModel>();
    
    public ImportWorkersWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        DataContext = null;
    }
    
    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.SlideIndex--;
        
        if (ViewModel.SlideIndex < 0)
        {
            ViewModel.SlideIndex = 0;
        }
    }

    private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.SlideIndex++;
        if (ViewModel.SlideIndex > 2)
        {
            ViewModel.SlideIndex = 2;
        }

        if (ViewModel.SlideIndex == 2)
        {
            ViewModel.PreProcessWorkers();
            ViewModel.ProcessWorkers();
        }
    }

    private void ButtonConfirmImport_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.ProcessWorkers();
        GlobalConstants.Config!.Data.Profile.Workers.AddRange(ViewModel.Workers);
        Close();
    }

    private void ButtonApplySettings_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.ProcessWorkers();
    }
}