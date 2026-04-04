using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Interface.Models.Duty;
using DynamicData.Kernel;
using Microsoft.Extensions.DependencyInjection;

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

    public bool IsPinyinAvailable => Plugin.IsPinyinAvailable;
    
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

    private async void InstallLibPinyinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        const string libPinyinPluginId = "lrs2187.LibPinyin";
        
        try
        {
            var pluginMarketService = IAppHost.GetService<IPluginMarketService>();
            
            var pluginInfo =
                pluginMarketService.MergedPlugins.FirstOrOptional(info => info.Value.Manifest.Id == libPinyinPluginId);
            if (!pluginInfo.HasValue)
            {
                pluginInfo.Value.Value.IsEnabled = true;
                this.ShowToast("已启用 LibPinyin 插件");
            }
            
            while (pluginMarketService.MergedPlugins
                   .All(plugin => plugin.Value.Manifest.Id != libPinyinPluginId))
            {
                await pluginMarketService.RefreshPluginSourceAsync();
            }

            pluginMarketService.RequestDownloadPlugin(libPinyinPluginId);
            this.ShowToast(new ToastMessage
            {
                Message = "已请求安装 LibPinyin",
                Duration = TimeSpan.FromSeconds(10)
            });

            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var uriNavigationService = IAppHost.GetService<IUriNavigationService>();
            uriNavigationService.Navigate(new Uri("classisland://app/settings/classisland.plugins"));
        }
        catch (Exception exception)
        {
            this.ShowErrorToast("在请求安装时出现错误", exception);
        }
    }
}