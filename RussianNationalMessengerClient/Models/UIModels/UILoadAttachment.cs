using System.Windows.Media;
using System.IO;

namespace RussianNationalMessengerClient.Models.UIModels;

public class UILoadAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FullPath { get; set; }
    public string FileName => Path.GetFileName(FullPath);
    public string Extension => Path.GetExtension(FullPath);
    public required Stream Stream { get; set; }
    public ImageSource? ImageSource { get; set; }
}
