using Microsoft.AspNetCore.Mvc;
using RussianNationalMessengerClient.Models.UIModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<UILoadAttachment> GetUILoadAttachmentAsync(string messageId, string attachmentId)
    {
        var loadDto = new
        {
            messageId,
            attachmentId
        };

        var json = JsonSerializer.Serialize(loadDto);

        using var request = new HttpRequestMessage(HttpMethod.Get, "File/GetFile")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            MessageBox.Show(problem?.Title ?? "ошибка");
        }

        return new()
        {
            Stream = await response.Content.ReadAsStreamAsync(),
            FullPath = response.Content.Headers.ContentType.MediaType,
            
        };

    }
}
