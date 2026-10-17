using RussianNationalMessengerClient.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace RussianNationalMessengerClient.Services;

public static class RequestToApiRNM
{
    private readonly static HttpClient _httpClient;

    static RequestToApiRNM()
    {
        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public static async Task<(LoginResponseDto? loginResponseDto, ProblemDetails? problem)> Login(string username, string password)
    {
        LoginDto login = new()
        {
            UserName = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7110/api/Login/login", login);

        if (!response.IsSuccessStatusCode)
            return (null, await response.Content.ReadFromJsonAsync<ProblemDetails>());

        var json = await response.Content.ReadAsStringAsync();

        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(json);

        if (loginResponse is null)
            return (null, new()
            {
                Title = "Сервер вернул некорректный ответ"
            });

        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", loginResponse.Token);

        return (loginResponse, null);
    }
}
