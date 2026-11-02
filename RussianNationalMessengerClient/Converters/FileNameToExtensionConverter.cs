using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;

namespace RussianNationalMessengerClient.Converters;

public class FileNameToExtensionConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
       Path.GetExtension(value.ToString());

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
