using RussianNationalMessengerClient.Models;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class LoginResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("user")]
    public Account User { get; set; }

    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}
