using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Message
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("chatId")]
    public string ChatId { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("isEdited")]
    public bool IsEdited { get; set; } = false;

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; } = false;

    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; } = null;

    [JsonIgnore]
    public string SentAtFormat => $"{SentAt.ToLocalTime():dd:MM:yyyy HH:mm:ss}";
}
