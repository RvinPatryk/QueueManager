using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace QueueManager.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            FieldInfo? field = value.GetType().GetField(value.ToString());

            if (field == null)
                return value.ToString() ?? string.Empty;

            DescriptionAttribute? attribute =
                field.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var field in targetType.GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();

                if ((attribute != null && attribute.Description == value.ToString())
                    || field.Name == value.ToString())
                {
                    return Enum.Parse(targetType, field.Name);
                }
            }

            return Binding.DoNothing;
        }
    }
}