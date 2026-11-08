using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

    public static BitmapImage? ConvertBytesToBitmapImage(byte[]? bytes)
    {
        try
        {
            BitmapImage bitmap = new();

            using MemoryStream stream = new(bytes);

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

    public static BitmapImage GetImageSourceFromPath(string path)
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

    public static byte[]? BitmapImageToBytes(BitmapImage image, string? extension = null)
    {

        extension = extension?.ToLower();
        try
        {
            BitmapEncoder encoder = extension switch
            {
                ".png" => new PngBitmapEncoder(),
                ".jpg" or ".jpeg" => new JpegBitmapEncoder() { QualityLevel = 90 },
                ".bmp" => new BmpBitmapEncoder(),
                ".gif" => new GifBitmapEncoder(),
                ".tif" or ".tiff" => new TiffBitmapEncoder(),
                ".wmp" => new WmpBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(image));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }
        catch
        {
            return null; 
        }
    }

    public static BitmapImage? ConvertStreamToBitmapImage(Stream stream)
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
