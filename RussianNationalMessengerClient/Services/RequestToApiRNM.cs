using RussianNationalMessengerClient.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

    public static async Task<LoginResponseDto> Login(string username, string password)
    {
        LoginDto login = new()
        {
            UserName = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7110/api/Login/login", login);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());

        var json = await response.Content.ReadAsStringAsync();

        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(json) ??
            throw new ArgumentNullException(nameof(json) + " is null");

        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", loginResponse.Token);

        return loginResponse;
    }
}
