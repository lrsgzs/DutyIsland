using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Shared;
using DutyIsland.Enums;
using DutyIsland.Shared;
using DutyIsland.ViewModels;
using DynamicData;

namespace DutyIsland.Views;

public partial class ImportWorkersWindow : ViewBase
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

    private void ButtonPrev_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SlideIndex--;

        if (ViewModel.SlideIndex < 0)
        {
            ViewModel.SlideIndex = 0;
        }
    }

    private void ButtonNext_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SlideIndex++;
        if (ViewModel.SlideIndex > 2)
        {
            ViewModel.SlideIndex = 2;
        }

        if (ViewModel.SlideIndex == 2)
        {
            ViewModel.Cleanup();
            if (ViewModel.ImportMode == ImportMode.Text)
            {
                ViewModel.PreProcessText();
            }
            ViewModel.PreProcessWorkers();
            ViewModel.ProcessWorkers();
        }
    }

    private async void OpenFile_OnClick(object? sender, RoutedEventArgs e)
    {
        var storage = TopLevel?.StorageProvider;
        if (storage == null) return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            Title = "选择表格文件",
            FileTypeFilter =
            [
                new FilePickerFileType("表格文件")
                {
                    Patterns = ["*.txt", "*.csv", "*.xlsx"],
                    MimeTypes =
                    [
                        "text/plain", "text/csv", "text/comma-separated-values",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    ],
                    AppleUniformTypeIdentifiers =
                    [
                        "public.plain-text", "public.comma-separated-values-text",
                        "org.openxmlformats.spreadsheetml.sheet"
                    ]
                }
            ]
        });
        
        if (files.Count == 0) return;
        var file = files[0];
        
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!new[] {".txt", ".xlsx", ".csv"}.Contains(extension))
        {
            this.ShowErrorToast("不支持的文件格式");
            return;
        }
        
        await using var stream = await file.OpenReadAsync();

        ViewModel.Sheet = extension switch
        {
            ".txt" => await LoadFromTxtAsync(stream),
            ".xlsx" => LoadFromExcelAsync(stream),
            ".csv" => LoadFromCsvAsync(stream),
            _ => []
        };

        ButtonNext_OnClick(sender, e);
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