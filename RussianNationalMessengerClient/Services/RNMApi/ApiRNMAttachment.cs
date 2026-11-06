using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Classes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Windows.Media;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.IO;
using System;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<UIMessageAttachment?> DownloadFile(string messageId, string attachmentId)
    {
        var dto = new
        {
            MessageId = messageId,
            AttachmentId = attachmentId
        };

        var response = await _httpClient.PostAsJsonAsync("File/getfile", dto);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine((int)response.StatusCode);
            ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Console.WriteLine(problem?.Title ?? "Ошибка загрузки файла");
            return null;
        }

        var stream = await response.Content.ReadAsStreamAsync();

        string? contentType = response.Content.Headers.ContentType?.ToString();

        string? fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName;

        fileName = fileName?.Trim('"');

        ImageSource? imageSource = null;

        if (contentType is not null)
            imageSource = ConvertImage.SupportedImageTypes.Contains(contentType.ToLower()) ? ConvertImage.ConvertStreamToImageSource(stream) : null;

        UIMessageAttachment model = new()
        {
            Id = attachmentId,
            Stream = stream,
            FileName = fileName,
            ContentType = contentType,
            ImageSource = imageSource
        };

        return model;
    }
}
