using DutyIsland.Services;

namespace DutyIsland.Shared;

/// <summary>
/// 公开常量
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// 插件路径
    /// </summary>
    public static string? PluginFolder { get; set; }
    
    /// <summary>
    /// 插件配置路径
    /// </summary>
    public static string? PluginConfigFolder { get; set; }

    /// <summary>
    /// 配置
    /// </summary>
    public static ConfigHandler? Config { get; set; }

    public static string PluginVersion { get; set; } = "???";

    public const string DutyPlanAttachedSettingsGuid = "4A0B491E-F6AF-431D-8D0C-B09AA8F5C661";
}