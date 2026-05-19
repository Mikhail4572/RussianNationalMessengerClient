using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class MessengerState : ViewModelBase
{
    private readonly Dictionary<string, ChatViewModel> _chatCache = new();

    public ObservableCollection<ChatViewModel> Chats { get; } = [];

    public ChatViewModel SelectedChat
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedChat));
        }
    }

    private readonly ServiceSignalR _service;
    private readonly NavigationService _navigation;

    public MessengerState(NavigationService navigation)
    {
        _navigation = navigation;
    }

    public ICommand ToChatPageCommand => new RelayCommand(_ => 
    {
        // переход к диалогу
    });

    public ChatViewModel? GetChat(string chatId)
    {
        _chatCache.TryGetValue(chatId, out ChatViewModel? chat);

        return chat;
    }

    public void LoadChats(List<Chat> chats)
    {
        Chats.Clear();

        _chatCache.Clear();

        foreach (Chat chat in chats)
        {
            ChatViewModel vm =
                new(chat);

            Chats.Add(vm);

            _chatCache.Add(chat.Id, vm);
        }
    }
}
