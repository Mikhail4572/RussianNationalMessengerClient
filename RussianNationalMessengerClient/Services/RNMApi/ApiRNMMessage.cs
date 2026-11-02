using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<(int statusCode, ProblemDetails? problem)> SendMessage(string chatId, string text, IEnumerable<string> filePaths)
    {
        using MultipartFormDataContent content = new();

        content.Add(new StringContent(chatId), "ChatId");
        content.Add(new StringContent(text), "Content");

        foreach (var item in filePaths)
        {
            FileStream fileStream = new(item, FileMode.Open, FileAccess.Read, FileShare.Read);

            // FileStream fileStream = File.OpenRead(item);

            StreamContent fileContent = new(fileStream);

            content.Add(fileContent, "Files", Path.GetFileName(item));
        }

        var response = await _httpClient.PostAsync("Message/SendMessage", content);

        if(!response.IsSuccessStatusCode)
            return ((int)response.StatusCode, await response.Content.ReadFromJsonAsync<ProblemDetails>());

        return ((int)response.StatusCode, null);
    }


}
