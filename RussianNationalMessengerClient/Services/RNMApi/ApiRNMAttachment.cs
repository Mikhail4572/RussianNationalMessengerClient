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
    public async Task<byte[]?> DownloadFile(string messageId, string attachmentId)
    {
        var response = await _httpClient.GetAsync($"File/getfile/{messageId}/{attachmentId}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine((int)response.StatusCode);
            ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Console.WriteLine(problem?.Title ?? "Ошибка загрузки файла");
            return null;
        }


        return await response.Content.ReadAsByteArrayAsync();
    }
}
