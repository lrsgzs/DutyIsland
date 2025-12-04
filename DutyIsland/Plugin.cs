using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using DutyIsland.Controls.ActionSettingsControls;
using DutyIsland.Controls.AttachedSettingsControls;
using DutyIsland.Controls.Components;
using DutyIsland.Controls.ComponentSettingsControls;
using DutyIsland.Services;
using DutyIsland.Services.Automations.Actions;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;
using DutyIsland.ViewModels.SettingPages;
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
        _logger.Info("DutyIsland  Copyright (C) 2025  lrs2187/lrsgzs\nThis program comes with ABSOLUTELY NO WARRANTY.\nThis is free software, and you are welcome to redistribute it under certain conditions.");
        _logger.Info("欢迎使用 DutyIsland！");
        _logger.Info("初期化中...");
        
        _logger.Info("加载配置...");
        GlobalConstants.PluginVersion = Info.Manifest.Version;
        GlobalConstants.PluginFolder = Info.PluginFolderPath;
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Config = new ConfigHandler();
        
        #if DEBUG
            _logger.Info("这是开发构建，遥测将会被关闭！");
        #else
            if (GlobalConstants.Config.Data.EnableSentry)
            {
                _logger.Info("遥测已启用! 感谢您的帮助~");
                _logger.Info("初始化 Sentry...");

                SentrySdk.Init(options =>
                {
                    options.Dsn = "https://c7689eb24b7f331dcca5d44960a0b974@o4510452375552000.ingest.us.sentry.io/4510452383612928";
                    options.Release = GlobalConstants.PluginVersion;
                    options.AutoSessionTracking = true;
                    options.Environment = GlobalConstants.Environment;
                });

                Dispatcher.UIThread.UnhandledException += (_, e) =>
                {
                    if (!IsDutyIslandException(e.Exception)) return;
                    
                    _logger.Error("出错了吗...交给 Sentry 吧！");
                    SentrySdk.CaptureException(e.Exception);
                };
            }
            else
            {
                _logger.Info("没开遥测吗...可惜了。");
            }
        #endif
        
        _logger.Info("注册服务...");
        services.AddSingleton<DutyPlanService>();
        
        _logger.Info("注册附加设置...");
        services.AddAttachedSettingsControl<DutyPlanAttachedSettingsControl>();
        
        _logger.Info("注册提醒提供方...");
        services.AddNotificationProvider<DutyNotificationProvider>();
        
        _logger.Info("注册视图模型...");
        services.AddTransient<DutyViewModel>();
        
        _logger.Info("注册设置页面...");
        services.AddSettingsPage<DutySettingsPage>();
        services.AddSettingsPage<DebugSettingsPage>();

        _logger.Info("注册组件...");
        services.AddComponent<DutyComponent, DutyComponentSettingsControl>();
        
        _logger.Info("注册行动...");
        services.AddAction<NotifyDutyAction, NotifyDutyActionSettingsControl>();
        
        // 应用启动
        AppBase.Current.AppStarted += (_, _) =>
        {
            _logger.Info("启动 DutyPlanService...");
            IAppHost.GetService<DutyPlanService>();
        };
        
        // 应用退出
        AppBase.Current.AppStopping += (_,_) =>
        {
            _logger.Info("兜底：保存全部配置...");
            GlobalConstants.Config.Save();
        };
    }

    private static bool IsDutyIslandException(Exception e)
    {
        if (e.StackTrace == null || e.StackTrace.Contains("dutyisland", StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return e.InnerException != null && IsDutyIslandException(e.InnerException);
    }
}

