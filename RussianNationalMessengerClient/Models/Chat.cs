using RussianNationalMessengerClient.ViewModels;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Chat : ViewModelBase
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("isGroup")]
    public bool IsGroup { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    // участники
    [JsonPropertyName("members")]
    public List<string> Members { get; set; }

    [JsonPropertyName("lastMessage")]
    public LastMessage? LastMessage
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(LastMessage));
        }
    }

    [JsonIgnore]
    public bool IsCreated { get; set; } = true;

    public override string ToString() => LastMessage?.Content ?? "";

}
