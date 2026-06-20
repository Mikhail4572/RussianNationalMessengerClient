using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class MessengerState : ViewModelBase
{
    private readonly ConcurrentDictionary<string, ChatViewModel> _chatCache = new();

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

            //_chatCache.Add(chat.Id, vm);

            _chatCache[chat.Id] = vm;

            Chats.Add(vm);
        }
    }
}
