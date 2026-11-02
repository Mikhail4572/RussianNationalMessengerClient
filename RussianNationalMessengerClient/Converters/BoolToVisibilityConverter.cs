using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RussianNationalMessengerClient.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(bool.TryParse(value.ToString(), out var flag))
            return flag ? Visibility.Visible : Visibility.Collapsed;

        return false;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
