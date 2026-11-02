using RussianNationalMessengerClient.Models.UIModels;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace RussianNationalMessengerClient.Converters;

public class ViewAttachmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not UILoadAttachment attachment)
            return null;

        if (attachment.ImageSource is not null)
        {
            Image image = new()
            {
                Source = attachment.ImageSource,
                Stretch = Stretch.Uniform,
                MaxHeight = 60
            };

            return image;
        }

        return attachment.Extension;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

}
