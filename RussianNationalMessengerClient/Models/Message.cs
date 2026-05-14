using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Message
{
    [Key]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("chatId")]
    public Guid ChatId { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; set; }

    [JsonPropertyName("isEdited")]
    public bool IsEdited { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}