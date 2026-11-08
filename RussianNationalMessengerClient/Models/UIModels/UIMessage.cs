
namespace RussianNationalMessengerClient.Models.UIModels;

public class UIMessage
{
    public string MessageId { get; set; }
    public string? Content { get; set; }
    public DateTime SentAt { get; set; }
    public string Author { get; set; }
    public string ChatId { get; set; }
    public List<UIMessageAttachment>? Attachments { get; set; }
}
