using System.Text;
using DotNetCommon.PinYin;

namespace DutyIsland.Shared;

public static class PinyinHelper
{
    public static readonly Dictionary<string, List<string>> FullPinyinCache = new();
    public static readonly Dictionary<string, List<string>> FirstPinyinCache = new();
    
    private static string AsString<T>(this IEnumerable<T> enumerable)
    {
        var builder = new StringBuilder();
        foreach (var item in enumerable)
        {
            builder.Append(item);
        }

        return builder.ToString();
    }
    
    private static IEnumerable<IEnumerable<T>> CrossMerge<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return _CrossMerge([], enumerable.Select(e => e.ToList()).ToList());
    }
    
    private static List<List<T>> _CrossMerge<T>(List<T> current, List<List<T>> enumerable)
    {
        var list = enumerable.ToList();
        var first = list.FirstOrDefault();

        if (first == null)
        {
            return [current];
        }

        var others = list[1..];
        List<List<T>> output = [];
        foreach (var nextEnumerable in first)
        {
            var copiedCurrent = current.Select(e => e).ToList();
            copiedCurrent.Add(nextEnumerable);
            output.AddRange(_CrossMerge(copiedCurrent, others));
        }

        return output;
    }

    public static List<string> GetAllPinyin(char c)
    {
        var allPinyin = WordsHelper.GetAllPinyin(c);
        return allPinyin?.Count > 0 ? allPinyin : [c.ToString()];
    }
    
    public static List<string> GetFullPinyinList(string text)
    {
        if (FullPinyinCache.TryGetValue(text, out var cachedResult))
        {
            return cachedResult;
        }
        
        var result = text
            .Select(GetAllPinyin)
            .CrossMerge()
            .Select(e => e.AsString())
            .Distinct()
            .ToList();

        FullPinyinCache[text] = result;
        return result;
    }
    
    public static List<string> GetFirstPinyinList(string text)
    {
        if (FirstPinyinCache.TryGetValue(text, out var cachedResult))
        {
            return cachedResult;
        }

        var result = text
            .Select(character =>
                GetAllPinyin(character)
                    .Select(pinyin => pinyin[0].ToString())
                    .ToArray())
            .CrossMerge()
            .Select(e => e.AsString())
            .Distinct()
            .ToList();

        FirstPinyinCache[text] = result;
        return result;
    }
}