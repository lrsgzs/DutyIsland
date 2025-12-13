using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClassIsland.Core.Controls;
using ClassIsland.Shared;
using DutyIsland.ViewModels;

namespace DutyIsland.Views;

public partial class ImportWorkersWindow : MyWindow
{
    public ImportWorkersViewModel ViewModel { get; } = IAppHost.GetService<ImportWorkersViewModel>();
    private bool _isOpened = false;
    
    public ImportWorkersWindow()
    {
        DataContext = this;
        InitializeComponent();
    }
    
    public void Open()
    {
        if (!_isOpened)
        {
            _isOpened = true;
            Show();
        }
        else
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            Activate();
        }
    }
}