using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CircuitSimulatorWpf.Services
{
    public class StringToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            if (value is string key)
            {
                return Application.Current.TryFindResource(key);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parametr, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
