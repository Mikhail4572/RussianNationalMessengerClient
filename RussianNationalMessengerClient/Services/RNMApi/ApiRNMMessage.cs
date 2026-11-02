using Microsoft.AspNetCore.Mvc;
using RussianNationalMessengerClient.Models.UIModels;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<(int statusCode, ProblemDetails? problem)> SendMessage(string chatId, string text, IEnumerable<UILoadAttachment> attachments)
    {
        using MultipartFormDataContent content = new();

        content.Add(new StringContent(chatId), "ChatId");
        content.Add(new StringContent(text), "Content");

        foreach (var item in attachments)
        {
            StreamContent fileContent = new(item.Stream);
            content.Add(fileContent, "Files", item.FileName);
        }

        var response = await _httpClient.PostAsync("Message/SendMessage", content);

        if(!response.IsSuccessStatusCode)
            return ((int)response.StatusCode, await response.Content.ReadFromJsonAsync<ProblemDetails>());

        return ((int)response.StatusCode, null);
    }


}
