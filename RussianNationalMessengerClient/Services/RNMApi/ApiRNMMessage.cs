using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<bool> DeleteMessage(string messageId) =>
         (await _httpClient.PostAsJsonAsync("Message/delete", messageId)).IsSuccessStatusCode;

    public async Task<(Message? returnMessage, ProblemDetails? problem)> SendMessage(string chatId, string? text, IEnumerable<string> paths = null)
    {
        try
        {
            using MultipartFormDataContent content = new();

            content.Add(new StringContent(chatId), "ChatId");
            content.Add(new StringContent(text ?? string.Empty), "Content");

            FileExtensionContentTypeProvider provider = new();

            foreach (var item in paths)
            {
                if (!File.Exists(item))
                    continue;

                var fileStream = File.OpenRead(item);

                StreamContent fileContent = new(fileStream);

                if (!provider.TryGetContentType(item, out string? contentType))
                    contentType = "application/octet-stream";

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                content.Add(fileContent, "Files", Path.GetFileName(item));
            }

            var response = await _httpClient.PostAsync("Message/send", content);

            if (!response.IsSuccessStatusCode)
            {
                return (null, await response.Content.ReadFromJsonAsync<ProblemDetails>());
            }

            Message? returnMessage = await response.Content.ReadFromJsonAsync<Message>();

            return (returnMessage, null);


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return (null, null);
        }
    }
}
