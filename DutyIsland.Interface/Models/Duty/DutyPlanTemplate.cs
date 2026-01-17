using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Duty;

/// <summary>
/// 值日表模板
/// </summary>
public partial class DutyPlanTemplate : ObservableObject
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [ObservableProperty] private string _templateName = "新值日表模板";
    
    /// <summary>
    /// 模板项字典，TKey 为模板项 Guid
    /// </summary>
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplateItem> _workerTemplateDictionary = new();
}