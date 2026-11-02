using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Models;

public class Attachment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonIgnore]
    public string GetExtension
    {
        get
        {
            int index = ContentType.IndexOf('/');

            if (index == -1)
                return "no content type";

            return ContentType[(index + 1)..];
        }
    }
}
