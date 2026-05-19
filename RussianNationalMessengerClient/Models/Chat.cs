using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Chat
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name 
    {
        get => string.IsNullOrEmpty(field) ? Members[1] : field;
        set;
    }
    
    [JsonPropertyName("isGroup")]
    public bool IsGroup { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    // участники
    [JsonPropertyName("members")]
    public List<string> Members { get; set; }

    [JsonPropertyName("lastMessage")]
    public LastMessage? LastMessage { get; set; }

    public override string ToString() => LastMessage?.Content ?? "";

}
