using RussianNationalMessengerClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class ChatMessagesDto
{
    [JsonPropertyName("chat")]
    public Chat Chat { get; set; }

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }
}
