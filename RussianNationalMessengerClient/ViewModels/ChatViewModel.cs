using RussianNationalMessengerClient.Models;
using System.Collections.ObjectModel;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatViewModel : ViewModelBase
{
    public Chat Chat { get; }

    public ObservableCollection<Message> Messages { get; set; } = [];

    public string DisplayName => Chat.Members.FirstOrDefault(x => x != _authState.CurrentUser.Id) ?? string.Empty;

    public string LastMessageContent
    {
        get
        {
            if(Chat.LastMessage is null)
                return string.Empty;

            return Chat.LastMessage.Content;
        }
    }

    public string LastMessageAuthor
    {
        get
        {
            if (Chat.LastMessage is null)
                return string.Empty;

            if (string.IsNullOrEmpty(Chat.Name))
                return Chat.LastMessage.Author == _authState.CurrentUser.Id ? "Вы:" : Chat.LastMessage.Author;

            return string.Empty;
        }
    }

    public void Update(Chat chat)
    {
        chat.LastMessage = chat.LastMessage;

        chat.Name = chat.Name;

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(LastMessageContent));
        OnPropertyChanged(nameof(LastMessageAuthor));
    }   

    public Message CurrentMessage
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentMessage));
        }
    } = new();

    private readonly AuthState _authState;

    public void LoadMessages(List<Message> messages)
    {
        Messages.Clear();

        foreach (Message message in messages)
            Messages.Add(message);
    }

    public ChatViewModel(Chat chat, AuthState authState)
    {
        Chat = chat;
        _authState = authState;
    }
}
