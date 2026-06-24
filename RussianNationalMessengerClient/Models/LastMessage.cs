using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class LastMessage
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("content")]
    public string Content
    {
        get => field.Replace("\r", " ").Replace("\n", "").Replace("\t", " ");
        set;
    }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; set; }

    public override string ToString() => Content;
}
