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
    private RegistrationResponseDto _responseDto;


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

    public async Task<RegistrationResponseDto> Register(string username, string email, string password)
    {
        var regDto = new
        {
            username,
            email,
            password
        };

        var response = await _httpClient.PostAsJsonAsync("Login/register", regDto);

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            throw new Exception("status code = " + response.StatusCode + "\nTitle problem = " + problem?.Title);
        }

        var responseDto = await response.Content.ReadFromJsonAsync<RegistrationResponseDto>();

        if (responseDto is null)
            throw new ArgumentNullException(nameof(responseDto));

        _responseDto = responseDto;

        return responseDto;
    }

    public async Task<LoginResponseDto?> VerifyRegistration(string code)
    {
        if (_responseDto is null or { TemporaryToken: null or "" })
            throw new ArgumentNullException(nameof(_responseDto));

        var verDto = new
        {
            _responseDto.TemporaryToken,
            code
        };

        var response = await _httpClient.PostAsJsonAsync("Login/verify-registration", verDto);
     
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            throw new Exception("status code = " + response.StatusCode + "\nTitle problem = " + problem?.Title);
        }

        var loginDto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        _responseDto = null;

        return loginDto;
    }
}
