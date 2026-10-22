using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Attachment
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("objectKey")]
    public string ObjectKey { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }
}
