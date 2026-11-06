using System.Windows.Media;
using System.IO;

namespace RussianNationalMessengerClient.Models.UIModels;

public class UIMessageAttachment
{
    public string Id { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
    public required Stream Stream { get; set; }
    public ImageSource? ImageSource { get; set; }
}
