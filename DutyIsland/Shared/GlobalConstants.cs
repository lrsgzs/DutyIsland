using DutyIsland.Services;

namespace DutyIsland.Shared;

/// <summary>
/// 公开常量
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// 插件配置路径
    /// </summary>
    public static string? PluginConfigFolder { get; set; }

    /// <summary>
    /// 配置
    /// </summary>
    public static ConfigHandler? Config { get; set; }
}