using Microsoft.AspNetCore.Mvc;
using RussianNationalMessengerClient.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30),
        BaseAddress = new("https://localhost:7110/api/")
    };

    public ApiRNM() { }
}
