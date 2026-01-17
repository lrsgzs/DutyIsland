using System.Text.Json.Serialization;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.ComponentModels;
using DutyIsland.Interface.Services;

namespace DutyIsland.Interface.Models.Duty;

/// <summary>
/// 值日表
/// </summary>
public partial class DutyPlan : ObservableObject
{
    /// <summary>
    /// 值日表名称
    /// </summary>
    [ObservableProperty] private string _name = "新值日表";
    
    /// <summary>
    /// 值日表模板 Guid
    /// </summary>
    private Guid _templateGuid = Guid.Empty;
    
    /// <summary>
    /// 执行者字典，TKey 为值日表模板项 Guid，TValue 为值日表项
    /// </summary>
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanItem> _workerDictionary = [];
    
    /// <summary>
    /// 值日表模板 Guid
    /// </summary>
    public Guid TemplateGuid
    {
        get => _templateGuid;
        set
        {
            if (_templateGuid == value)
            {
                return;
            }
            
            _templateGuid = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Template));
            OnPropertyChanged(nameof(ComplexItems));
        }
    }
    
    /// <summary>
    /// 值日表模板
    /// </summary>
    [JsonIgnore]
    public DutyPlanTemplate? Template => IDutyPlanService.GetService().Profile.DutyPlanTemplates.GetValueOrDefault(TemplateGuid);

    /// <summary>
    /// 值日表复合项，用于同时获取值日表项和值日表模板项
    /// </summary>
    [JsonIgnore]
    public SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? ComplexItems
    {   
        get
        {
            if (IDutyPlanService.GetService().Profile.DutyPlanTemplates.TryGetValue(TemplateGuid, out var template))
            {
                return new SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>(
                    template.WorkerTemplateDictionary
                        .Select(e =>
                        {
                            var item = WorkerDictionary.GetValueOrDefault(e.Key, new DutyPlanItem());
                            return KeyValuePair.Create(e.Key, new ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>(item, e.Value));
                        })
                        .ToDictionary(),
                    Guid.NewGuid);
            }

            return null;
        }
    }
}