using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Chat
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name 
    {
        get => string.IsNullOrEmpty(field) ? Members[0] : Name;
        set;
    }
    
    [JsonPropertyName("isGroup")]
    public bool IsGroup { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    // участники
    [JsonPropertyName("members")]
    public List<string> Members { get; set; }
}
