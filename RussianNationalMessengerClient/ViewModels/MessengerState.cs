using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace RussianNationalMessengerClient.ViewModels;

public class MessengerState : ViewModelBase
{
    //key - chatId value - ChatViewModel
    private readonly ConcurrentDictionary<string, ChatViewModel> _chatCache = new();

    // список всех чатов
    public ObservableCollection<ChatViewModel> Chats { get; } = [];

    public Account? SelectedAccount
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedAccount));
        }
    }

    public ChatViewModel? SelectedChat
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedChat));
        }
    }

    private readonly AuthState _authState;
    private readonly ChatViewModelFactory _factory;

    public MessengerState(AuthState authState, ChatViewModelFactory factory)
    {
        _authState = authState;
        _factory = factory;
    }

    public bool RemoveChat(string chatId) =>
        _chatCache.TryRemove(chatId, out _);    

    public ChatViewModel? GetChat(string chatId)
    {
        _chatCache.TryGetValue(chatId, out ChatViewModel? chat);
        return chat;
    }

    public void LoadChats(List<Chat> chats)
    {
        foreach (Chat chat in chats)
        {
            if (_chatCache.TryGetValue(chat.Id, out var existingChat))
            {
                existingChat.Update(chat);
                continue;
            }
            
            ChatViewModel vm = _factory.Create(chat);

            _chatCache[chat.Id] = vm;

            Chats.Add(vm);
        }
    }
}
