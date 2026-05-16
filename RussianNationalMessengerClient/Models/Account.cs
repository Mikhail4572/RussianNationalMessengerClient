using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Account
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("isOnline")]
    public bool IsOnline { get; set; }
}
