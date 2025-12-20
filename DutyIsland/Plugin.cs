using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services;
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
using DutyIsland.ViewModels;
using DutyIsland.ViewModels.SettingPages;
using DutyIsland.Views;
using DutyIsland.Views.SettingPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;
using SuperAutoIsland.Interface.Services;

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
        
        _logger.Info("注册视图模型...");
        services.AddTransient<DutyViewModel>();
        services.AddTransient<ImportWorkersViewModel>();
        
        _logger.Info("注册页面...");
        services.AddSettingsPage<DutySettingsPage>();
        services.AddSettingsPage<DebugSettingsPage>();
        
        _logger.Info("注册 ClassIsland 元素...");
        services.AddAttachedSettingsControl<DutyPlanAttachedSettingsControl>();
        services.AddNotificationProvider<DutyNotificationProvider>();
        services.AddComponent<DutyComponent, DutyComponentSettingsControl>();
        services.AddAction<NotifyDutyAction, NotifyDutyActionSettingsControl>();
        
        // 应用启动
        AppBase.Current.AppStarted += (_, _) =>
        {
            _logger.Info("启动 DutyPlanService...");
            var dutyPlanService = IAppHost.GetService<DutyPlanService>();

            if (IsPluginInstalled("lrs2187.sai", new Version(0, 1, 2, 4)))
            {
                var saiServerService = IAppHost.GetService<ISaiServer>();

                _logger.Info("注册 SuperAutoIsland 元素...");
                saiServerService.RegisterBlocks("DutyIsland", new RegisterData
                {
                    Actions =
                    [
                        new BlockMetadata
                        {
                            Id = "duty.actions.notifyDuty",
                            Name = "提醒值日人员",
                            Icon = ("日历助手", "\uE314"),
                            Args = new Dictionary<string, MetaArgsBase>
                            {
                                ["JobGuid"] = new DropDownMetaArgs
                                {
                                    Name = "",
                                    Type = MetaType.dropdown,
                                    Options = EnsureListHasItemOrDefaultListItem(
                                        dutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary
                                            .Select(e => (e.Value.Name, e.Key.ToString()))
                                            .ToList(),
                                        new ValueTuple<string, string>("???", GlobalConstants.TemplateNullGuid.ToString()))
                                },
                                ["FallbackSettings.Enabled"] = new CheckboxMetaArgs
                                {
                                    Name = "启用回滚?",
                                    Type = MetaType.checkbox,
                                    DefaultValue = false
                                },
                                ["FallbackSettings.JobName"] = new CommonMetaArgs
                                {
                                    Name = "回滚任务名称",
                                    Type = MetaType.text
                                },
                                ["UseCustomNotificationSettings"] = new CheckboxMetaArgs
                                {
                                    Name = "启用自定义提醒设置?",
                                    Type = MetaType.checkbox,
                                    DefaultValue = false
                                },
                                ["CustomNotificationSettings.Title"] = new CommonMetaArgs
                                {
                                    Name = "遮罩内容",
                                    Type = MetaType.text
                                },
                                ["CustomNotificationSettings.TitleDuration"] = new CommonMetaArgs
                                {
                                    Name = "遮罩时长",
                                    Type = MetaType.number
                                },
                                ["CustomNotificationSettings.TitleEnableSpeech"] = new CommonMetaArgs
                                {
                                    Name = "启用遮罩语音?",
                                    Type = MetaType.boolean
                                },
                                ["CustomNotificationSettings.Text"] = new CommonMetaArgs
                                {
                                    Name = "正文内容",
                                    Type = MetaType.text
                                },
                                ["_string0"] = new CommonMetaArgs
                                {
                                    Name = "说明:",
                                    Type = MetaType.dummy
                                },
                                ["_string1"] = new CommonMetaArgs
                                {
                                    Name = "%n - 履行该任务的成员",
                                    Type = MetaType.dummy
                                },
                                ["_string2"] = new CommonMetaArgs
                                {
                                    Name = "%j - 该任务名称",
                                    Type = MetaType.dummy
                                },
                                ["CustomNotificationSettings.TextDuration"] = new CommonMetaArgs
                                {
                                    Name = "正文时长",
                                    Type = MetaType.number
                                },
                                ["CustomNotificationSettings.TextEnableSpeech"] = new CommonMetaArgs
                                {
                                    Name = "启用正文语音?",
                                    Type = MetaType.boolean
                                }
                            },
                            DropdownUseNumbers = false,
                            InlineField = false,
                            InlineBlock = false,
                            IsRule = false
                        }
                    ],
                    Rules = []
                });
            }
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
        if (e.StackTrace?.Contains("dutyisland", StringComparison.CurrentCultureIgnoreCase) ?? false)
        {
            return true;
        }

        return e.InnerException != null && IsDutyIslandException(e.InnerException);
    }

    private static bool IsPluginInstalled(string pkgName, Version? version = null)
    {
        return IPluginService.LoadedPlugins.Any(info => info.Manifest.Id == pkgName && new Version(info.Manifest.Version) >= version);
    }

    private static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T>? data, T defaultItem)
    {
        return data?.Count > 0 ? data : [defaultItem];
    }
}

