using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class RegistrationResponseDto
{
    [JsonPropertyName("requiresVerification")]
    public bool RequiresVerification { get; set; }

    [JsonPropertyName("temporaryToken")]
    public string TemporaryToken { get; set; }
}
