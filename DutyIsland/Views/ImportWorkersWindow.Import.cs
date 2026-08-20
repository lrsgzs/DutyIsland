using System.Text;
using MiniExcelLibs;
using MiniExcelLibs.Csv;
using MiniExcelLibs.OpenXml;

namespace DutyIsland.Views;

public partial class ImportWorkersWindow
{
    private static async Task<List<List<string>>> LoadFromTxtAsync(Stream stream)
    {
        var content = Encoding.UTF8.GetString(await ReadAllBytesAsync(stream));
        using var reader = new StringReader(content);
        
        List<List<string>> lines = [];
        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add([line.Trim()]);
        }
        
        return lines;
    }

    private static List<List<string>> LoadFromCsvAsync(Stream stream)
    {
        return GetExcelList(stream.Query(configuration: new CsvConfiguration(), excelType: ExcelType.CSV));
    }

    private static List<List<string>> LoadFromExcelAsync(Stream stream)
    {
        var config = new OpenXmlConfiguration
        {
            FillMergedCells = true
        };
        return GetExcelList(stream.Query(configuration: config));
    }

    private static List<List<string>> GetExcelList(IEnumerable<dynamic> excel)
    {
        return excel
            .Select(row => (IDictionary<string, object?>)row)
            .Select(dict => dict
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value?.ToString() ?? "")
                .ToList())
            .ToList();
    }
    
    private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
    {
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream.ToArray();
    }
}
