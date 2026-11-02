using Microsoft.AspNetCore.Mvc;
using RussianNationalMessengerClient.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<(LoginResponseDto? loginResponseDto, ProblemDetails? problem)> Login(string username, string password)
    {
        LoginDto login = new()
        {
            UserName = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("Login/login", login);

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
