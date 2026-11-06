using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class LoginDto
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "mikhail";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "1111";
}
