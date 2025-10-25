using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using DutyIsland.Services;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;
using DutyIsland.Views.SettingPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    private readonly Logger _logger = new("DutyIsland");
    
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        _logger.Info("欢迎使用 DutyIsland");
        _logger.Info("初期化中...");

        _logger.Info("加载配置...");
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Config = new ConfigHandler();
        
        _logger.Info("添加设置页面...");
        services.AddSettingsPage<DutySettingsPage>();
        
        // 应用退出
        AppBase.Current.AppStopping += (_,_) =>
        {
            _logger.Info("兜底：保存全部配置...");
            GlobalConstants.Config.Save();
        };
    }
}

