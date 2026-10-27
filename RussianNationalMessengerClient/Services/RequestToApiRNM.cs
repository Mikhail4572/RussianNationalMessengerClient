using RussianNationalMessengerClient.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http;

namespace RussianNationalMessengerClient.Services;

public class RequestToApiRNM
{
    private readonly HttpClient _httpClient;

    public RequestToApiRNM()
    {
        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<(LoginResponseDto? loginResponseDto, ProblemDetails? problem)> Login(string username, string password)
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
