using RussianNationalMessengerClient.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Message
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("chatId")]
    public string ChatId { get; set; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; set; }

    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    [JsonIgnore]
    public string SentAtFormat => $"{SentAt.ToLocalTime():dd:MM:yyyy HH:mm:ss}";
}
