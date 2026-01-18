using System.Reflection;
using DutyIsland.Services;
using DutyIsland.Services.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DutyIsland.Shared;

/// <summary>
/// 公开常量
/// </summary>
public static class GlobalConstants
{
    public static ConfigHandler? Config { get; set; }
    
    public static ILoggerFactory LoggingFactory { get; } = LoggerFactory.Create(builder =>
    {
        builder.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
        builder.AddConsole(console => { console.FormatterName = "classisland"; });
        builder.SetMinimumLevel(LogLevel.Trace);
    });
    
    public static class Information
    {
        public static string PluginFolder { get; set; } = string.Empty;
        public static string PluginConfigFolder { get; set; } = string.Empty;
        public static string PluginVersion { get; set; } = "???";
    
#if DEBUG
        public static string Environment { get; } = "development ci-" + Assembly.GetAssembly(typeof(ClassIsland.Shared.IAppHost))?.GetName().Version;
#else
        public static string Environment { get; } = "production ci-" + Assembly.GetAssembly(typeof(ClassIsland.Shared.IAppHost))?.GetName().Version;
#endif
    }

    public static class Documents
    {
        public static string Readme { get; set; } = string.Empty;
    }
    
    public static class Guids
    {
        public static readonly Guid NullTemplate = Guid.Parse("8ca34af7-03cb-47ab-a630-0083dc942135");
        public const string DutyPlanAttachedSettings = "4A0B491E-F6AF-431D-8D0C-B09AA8F5C661";
        public const string DutyNotificationProvider = "CA0B77B2-FBC3-449F-A14D-B6D4EAA2726C";
        public const string DutyActionNotificationChannel = "881AAD3D-26FD-4FCF-B6E2-D39A996C59AC";
        public const string DutyAutoNotificationChannel = "7B973AB0-98EE-41AA-BD67-464B8EB011B1";
        public const string DutyTaskBarNotificationChannel = "88C27C28-FC9E-418F-8A70-AC8D353F2D2C";
    }

    public static class ImportSheetStaticTexts
    {
        public static readonly List<string> NameHeaderTexts = ["姓名", "名字", "name"];
        public static readonly List<string> IdHeaderTexts = ["编号", "学号", "考号", "id"];
        public static readonly List<string> SexHeaderTexts = ["性别", "sex"];

        public static class SexTexts
        {
            public static readonly List<string> Male = ["男", "male", "boy", "man", "1"];
            public static readonly List<string> Female = ["女", "female", "girl", "woman", "0"];
        }
    }
}