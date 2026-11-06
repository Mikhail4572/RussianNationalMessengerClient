using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RussianNationalMessengerClient.Classes;

public static class ConvertImage
{
    public static readonly IReadOnlyList<string> SupportedImageTypes = 
    [   
       // "image/jpg",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp",
        "image/tiff",
        "image/vnd.microsoft.icon",
        "image/x-icon"
    ];

    public static ImageSource GetImageSourceFromPath(string path)
    {
        try
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public static ImageSource? ConvertStreamToImageSource(Stream stream)
    {
        try
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            bitmap.Freeze();

            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}
