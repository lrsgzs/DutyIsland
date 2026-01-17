using System.Globalization;
using Avalonia.Data.Converters;
using DutyIsland.Interface.Enums;

namespace DutyIsland.Converters;

public class HumanSexToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is HumanSex sex)
        {
            return sex switch
            {
                HumanSex.Male => "男",
                HumanSex.Female => "女",
                _ => "未知"
            };
        }

        return "未知";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}