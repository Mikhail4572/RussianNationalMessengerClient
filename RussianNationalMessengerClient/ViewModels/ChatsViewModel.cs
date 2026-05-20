using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatsViewModel : ViewModelBase
{
    private readonly MessengerState _messengerState;

    private readonly ServiceSignalR _signalR;

    public ObservableCollection<ChatViewModel> Chats => _messengerState.Chats;

    public ChatViewModel? SelectedChat
    {
        get => _messengerState.SelectedChat;

        set
        {
            _messengerState.SelectedChat = value;

            OnPropertyChanged(nameof(SelectedChat));

            if (value is not null)
            {
                _ = LoadMessagesAsync(value);
            }
        }
    }

    public ChatsViewModel(MessengerState messengerState, ServiceSignalR signalR)
    {
        _messengerState = messengerState;
        _signalR = signalR;
    }

    private async Task LoadMessagesAsync(ChatViewModel chat) =>
        await _signalR.GetMessagesAsync(chat.Chat.Id);

}
