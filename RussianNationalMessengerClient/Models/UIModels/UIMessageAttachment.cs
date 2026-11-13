using RussianNationalMessengerClient.ViewModels;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RussianNationalMessengerClient.Models.UIModels;

public class UIMessageAttachment : ViewModelBase
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string SizeFormat
    {
        get
        {
            if (Size < 1024)
                return $"{Size} Б";

            if (Size < 1024 * 1024)
                return $"{Size / 1024.0:F1} КБ";

            return $"{Size / (1024.0 * 1024):F1} МБ";
        }
    }

    public bool IsImage => ContentType?.StartsWith("image/") == true;

    public BitmapImage? Preview
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Preview));
        }
    }
}
