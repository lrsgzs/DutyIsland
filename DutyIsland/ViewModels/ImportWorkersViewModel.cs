using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Enums;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Shared;

namespace DutyIsland.ViewModels;

public partial class ImportWorkersViewModel : ObservableRecipient
{
    /// <summary>
    /// 列信息
    /// </summary>
    public partial class ColumnInfo : ObservableRecipient
    {
        [ObservableProperty] private string _header = string.Empty;
        [ObservableProperty] private int _index = 0;
    }

    public record HeaderItem
    {
        public string HeaderText { get; init; } = string.Empty;
        public string IndexText { get; init; } = string.Empty;
    }
    
    // 视图状态
    [ObservableProperty] private int _slideIndex = 0;
    
    // 数据
    [ObservableProperty] private string _importSheetText = string.Empty;
    [ObservableProperty] private ObservableCollection<HeaderItem> _sheetHeaders = [];
    private List<List<string>> _sheet = [];
    
    [ObservableProperty] private bool _hasSheetHeader = false;
    [ObservableProperty] private bool _hasNameColumn = false;
    [ObservableProperty] private bool _hasIdColumn = false;
    [ObservableProperty] private bool _hasSexColumn = false;
    
    [ObservableProperty] private ColumnInfo _nameColumnInfo = new();
    [ObservableProperty] private ColumnInfo _idColumnInfo = new();
    [ObservableProperty] private ColumnInfo _sexColumnInfo = new();
    
    // 结果
    [ObservableProperty] private ObservableCollection<WorkerItem> _workers = [];
    
    private void Cleanup()
    {
        HasSheetHeader = false;
        HasNameColumn = false;
        HasIdColumn = false;
        HasSexColumn = false;

        NameColumnInfo = new ColumnInfo();
        IdColumnInfo = new ColumnInfo();
        SexColumnInfo = new ColumnInfo();
        
        _sheet.Clear();
        SheetHeaders.Clear();
        Workers.Clear();
    }
    
    /// <summary>
    /// 预处理
    /// </summary>
    public void PreProcessWorkers()
    {
        Cleanup();
        
        // 查询是否有内容
        var sheetLines = ImportSheetText.Split("\n").ToList();
        if (sheetLines.Count == 0)
        {
            return;
        }
        
        // 判断是否含有表头
        List<List<string>> headerChoices =
        [
            GlobalConstants.ImportSheetStaticTexts.NameHeaderTexts,
            GlobalConstants.ImportSheetStaticTexts.IdHeaderTexts,
            GlobalConstants.ImportSheetStaticTexts.SexHeaderTexts
        ];
        foreach (var choice in from list in headerChoices from choice in list select choice)
        {
            if (!sheetLines[0].Contains(choice)) continue;
            
            HasSheetHeader = true;
            break;
        }

        // 填表
        foreach (var line in sheetLines)
        {
            _sheet.Add(line.Split("\t").Select(item => item.Trim()).ToList());
        }

        // 表头
        for (var i = 0; i < _sheet[0].Count; i++)
        {
            SheetHeaders.Add(new HeaderItem
            {
                HeaderText = _sheet[0][i],
                IndexText = $"第 {i} 列"
            });
        }

        if (!HasSheetHeader)
        {
            // 晚会再做自动识别列内容
            
            return;
        }
        
        // 识别列标题
        for (var i = 0; i < _sheet[0].Count; i++)
        {
            foreach (var choice in GlobalConstants.ImportSheetStaticTexts.NameHeaderTexts.Where(choice => _sheet[0][i] == choice))
            {
                HasNameColumn = true;
                NameColumnInfo.Header = choice;
                NameColumnInfo.Index = i;
                break;
            }
            
            foreach (var choice in GlobalConstants.ImportSheetStaticTexts.IdHeaderTexts.Where(choice => _sheet[0][i] == choice))
            {
                HasIdColumn = true;
                IdColumnInfo.Header = choice;
                IdColumnInfo.Index = i;
                break;
            }

            foreach (var choice in GlobalConstants.ImportSheetStaticTexts.SexHeaderTexts.Where(choice => _sheet[0][i] == choice))
            {
                HasSexColumn = true;
                SexColumnInfo.Header = choice;
                SexColumnInfo.Index = i;
                break;
            }
        }
    }
    
    public void ProcessWorkers()
    {
        if (!HasNameColumn && !HasIdColumn)
        {
            return;
        }
        
        Workers.Clear();

        for (var i = 0; i < _sheet.Count; i++)
        {
            // 跳过表头
            if (HasSheetHeader && i == 0)
            {
                continue;
            }
            
            // 跳过空行
            var line = _sheet[i];
            if (line.Count == 0 || line is [""])
            {
                return;
            }
            
            // 识别字段
            var name = string.Empty;
            var id = string.Empty;
            var sex = HumanSex.Unknown;
            
            if (HasNameColumn)
            {
                name = line[NameColumnInfo.Index];
            }

            if (HasIdColumn)
            {
                id = line[IdColumnInfo.Index];
            }

            if (HasSexColumn)
            {
                if (GlobalConstants.ImportSheetStaticTexts.SexTexts.Male.Any(choice => line[SexColumnInfo.Index] == choice))
                {
                    sex = HumanSex.Male;
                }
                else if (GlobalConstants.ImportSheetStaticTexts.SexTexts.Female.Any(choice => line[SexColumnInfo.Index] == choice))
                {
                    sex = HumanSex.Female;
                }
            }
            
            // 添加
            Workers.Add(new WorkerItem
            {
                Name = name,
                Id = id,
                Sex = sex
            });
        }
    }
}