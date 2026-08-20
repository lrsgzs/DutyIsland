using System.Collections.Specialized;
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

    private SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? _complexItems;
    private ObservableOrderedDictionary<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? _complexItemsSource;
    private DutyPlanTemplate? _complexItemsTemplate;

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

            DetachComplexItemsEvents();
            _templateGuid = value;
            _complexItems = null;
            _complexItemsSource = null;
            _complexItemsTemplate = null;
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
            if (!IDutyPlanService.GetService().Profile.DutyPlanTemplates.TryGetValue(TemplateGuid, out var template))
            {
                DetachComplexItemsEvents();
                _complexItems = null;
                _complexItemsSource = null;
                _complexItemsTemplate = null;
                return null;
            }

            if (_complexItems == null || !ReferenceEquals(_complexItemsTemplate, template))
            {
                DetachComplexItemsEvents();
                _complexItemsTemplate = template;
                _complexItemsSource = new ObservableOrderedDictionary<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>();
                _complexItems = new SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>(
                    _complexItemsSource, Guid.NewGuid);

                template.WorkerTemplateDictionary.CollectionChanged += OnWorkerTemplateDictionaryChanged;
                WorkerDictionary.CollectionChanged += OnWorkerDictionaryChanged;

                RebuildComplexItems();
            }

            return _complexItems;
        }
    }

    private void RebuildComplexItems()
    {
        if (_complexItemsSource == null || _complexItemsTemplate == null)
        {
            return;
        }

        _complexItemsSource.Clear();
        foreach (var kvp in _complexItemsTemplate.WorkerTemplateDictionary)
        {
            _complexItemsSource.Add(
                kvp.Key,
                new ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>(
                    WorkerDictionary.GetValueOrDefault(kvp.Key, new DutyPlanItem()),
                    kvp.Value));
        }
    }

    private void DetachComplexItemsEvents()
    {
        if (_complexItemsTemplate != null)
        {
            _complexItemsTemplate.WorkerTemplateDictionary.CollectionChanged -= OnWorkerTemplateDictionaryChanged;
        }

        WorkerDictionary.CollectionChanged -= OnWorkerDictionaryChanged;
        _complexItemsTemplate = null;
    }

    private void OnWorkerTemplateDictionaryChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_complexItemsSource == null || _complexItemsTemplate == null)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems == null)
                {
                    break;
                }

                foreach (KeyValuePair<Guid, DutyPlanTemplateItem> kvp in e.NewItems)
                {
                    if (!_complexItemsSource.ContainsKey(kvp.Key))
                    {
                        _complexItemsSource.Add(
                            kvp.Key,
                            new ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>(
                                WorkerDictionary.GetValueOrDefault(kvp.Key, new DutyPlanItem()),
                                kvp.Value));
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                {
                    break;
                }

                foreach (KeyValuePair<Guid, DutyPlanTemplateItem> kvp in e.OldItems)
                {
                    _complexItemsSource.Remove(kvp.Key);
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems == null)
                {
                    break;
                }

                foreach (KeyValuePair<Guid, DutyPlanTemplateItem> kvp in e.NewItems)
                {
                    if (_complexItemsSource.TryGetValue(kvp.Key, out var tuple))
                    {
                        tuple.Second = kvp.Value;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                RebuildComplexItems();
                break;
        }
    }

    private void OnWorkerDictionaryChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_complexItemsSource == null)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems == null)
                {
                    break;
                }

                foreach (KeyValuePair<Guid, DutyPlanItem> kvp in e.NewItems)
                {
                    if (_complexItemsSource.TryGetValue(kvp.Key, out var tuple))
                    {
                        tuple.First = kvp.Value;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                {
                    break;
                }

                foreach (KeyValuePair<Guid, DutyPlanItem> kvp in e.OldItems)
                {
                    if (_complexItemsSource.TryGetValue(kvp.Key, out var tuple))
                    {
                        tuple.First = new DutyPlanItem();
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                RebuildComplexItems();
                break;
        }
    }
}
