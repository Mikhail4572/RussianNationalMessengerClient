using System.Net.Http;

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
