// ReSharper disable once RedundantUsingDirective
using System.Reflection;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using DutyIsland.Controls.ActionSettingsControls;
using DutyIsland.Controls.AttachedSettingsControls;
using DutyIsland.Controls.Components;
using DutyIsland.Controls.ComponentSettingsControls;
using DutyIsland.Interface.Services;
using DutyIsland.Services;
using DutyIsland.Services.Automations;
using DutyIsland.Services.Automations.Actions;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using DutyIsland.ViewModels;
using DutyIsland.ViewModels.SettingPages;
using DutyIsland.Views.SettingPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DutyIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public static bool IsPinyinAvailable = false;
    private ILogger<Plugin> _logger = GlobalConstants.LoggingFactory.CreateLogger<Plugin>();
    
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        _logger.LogInformation("欢迎使用 DutyIsland！初期化中...");
        _logger.LogInformation("版本号 {Version} 环境 {Environment}", Info.Manifest.Version, GlobalConstants.Information.Environment);
        
        _logger.LogInformation("加载配置...");
        GlobalConstants.Information.PluginVersion = Info.Manifest.Version;
        GlobalConstants.Information.PluginFolder = Info.PluginFolderPath;
        GlobalConstants.Information.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Config = new ConfigHandler();
        GlobalConstants.Documents.Readme = File.ReadAllText(Path.Combine(Info.PluginFolderPath, "README.md"));
        
        #if DEBUG
            _logger.LogInformation("这是开发构建，遥测将会被关闭！");
        #else
            if (GlobalConstants.Config.Data.EnableSentry)
            {
                _logger.LogInformation("遥测已启用! 感谢您的帮助~");
                _logger.LogInformation("初始化 Sentry...");

                SentrySdk.Init(options =>
                {
                    options.Dsn = "https://c7689eb24b7f331dcca5d44960a0b974@o4510452375552000.ingest.us.sentry.io/4510452383612928";
                    options.Release = GlobalConstants.Information.PluginVersion;
                    options.AutoSessionTracking = true;
                    options.SendClientReports = false;
                    options.IsGlobalModeEnabled = true;
                    options.Environment = GlobalConstants.Information.Environment;
                    options.TracesSampleRate = 0.05;
                    options.SetBeforeSend(sentryEvent =>
                    {
                        if (sentryEvent.Exception is { } exception && !IsDutyIslandException(exception))
                        {
                            return null;
                        }

                        return sentryEvent;
                    });
                });

                Dispatcher.UIThread.UnhandledException += (_, e) =>
                {
                    if (!IsDutyIslandException(e.Exception)) return;
                    
                    _logger.LogError("出错了吗...交给 Sentry 吧！");
                    SentrySdk.CaptureException(e.Exception);
                };
            }
            else
            {
                _logger.LogInformation("没开遥测吗...可惜了。");
            }
        #endif
        
        _logger.LogInformation("注册服务...");
        services.AddSingleton<IDutyPlanService, DutyPlanService>();
        services.AddSingleton<DutyTaskBarMenuService>();
        
        _logger.LogInformation("注册视图模型...");
        services.AddTransient<DutyViewModel>();
        services.AddTransient<ImportWorkersViewModel>();
        
        _logger.LogInformation("注册 ClassIsland 元素...");
        
        // SettingsPage
        services.AddSettingsPageGroup("duty.settings", FluentIcons.BroomRegular, "DutyIsland 值日表");
        services.AddSettingsPage<DutyMainSettingsPage>();
        services.AddSettingsPage<DutyProfileSettingsPage>();
        services.AddSettingsPage<DutyRollingSettingsPage>();
        services.AddSettingsPage<DebugSettingsPage>();
        
        // Others
        services.AddAttachedSettingsControl<DutyPlanAttachedSettingsControl>();
        services.AddNotificationProvider<DutyNotificationProvider>();
        services.AddComponent<DutyComponent, DutyComponentSettingsControl>();
        services.AddAction<NotifyDutyAction, NotifyDutyActionSettingsControl>();

        if (GlobalConstants.Config.Data.EnableExperimentFeature)
        {
            services.AddAction<NotifyAllJobAction>();
        }
        
        // 应用启动
        AppBase.Current.AppStarted += (_, _) =>
        {
            _logger = IAppHost.GetService<ILogger<Plugin>>();
            
            _logger.LogInformation("DutyIsland 晚加载...");
            GlobalConstants.Config.Logger = IAppHost.GetService<ILogger<ConfigHandler>>();
            
            _logger.LogInformation("启动服务...");
            IAppHost.GetService<IDutyPlanService>();
            IAppHost.GetService<DutyTaskBarMenuService>();

            if (IsPluginInstalled("lrs2187.sai", new Version("0.1.2.4")))
            {
                _logger.LogInformation("注册 SuperAutoIsland 元素...");
                AutomationsRegister.RegisterSai();
            }

            IsPinyinAvailable = IsPluginInstalled("lrs2187.LibPinyin");
        };
        
        // 应用退出
        AppBase.Current.AppStopping += (_,_) =>
        {
            _logger.LogInformation("兜底：保存全部配置...");
            GlobalConstants.Config.Save();
        };
    }

    private static readonly Assembly DutyIslandAssembly = typeof(Plugin).Assembly;
    private static readonly Assembly DutyIslandInterfaceAssembly = typeof(IDutyPlanService).Assembly;

    private static bool IsDutyIslandException(Exception e)
    {
        if (HasDutyIslandFrame(e))
        {
            return true;
        }

        return e.InnerException != null && IsDutyIslandException(e.InnerException);
    }

    private static bool HasDutyIslandFrame(Exception e)
    {
        if (string.IsNullOrEmpty(e.StackTrace))
        {
            return false;
        }

        var stackTrace = new System.Diagnostics.StackTrace(e);
        foreach (var frame in stackTrace.GetFrames() ?? [])
        {
            var assembly = frame.GetMethod()?.DeclaringType?.Assembly;
            if (assembly == DutyIslandAssembly || assembly == DutyIslandInterfaceAssembly)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPluginInstalled(string pkgName, Version? version = null)
    {
        return IPluginService.LoadedPlugins.Any(info => info.Manifest.Id == pkgName && info.IsEnabled && new Version(info.Manifest.Version) >= version);
    }

    internal static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T>? data, T defaultItem)
    {
        return data?.Count > 0 ? data : [defaultItem];
    }
}

