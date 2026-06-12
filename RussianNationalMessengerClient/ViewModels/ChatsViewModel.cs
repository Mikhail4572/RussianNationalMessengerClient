using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

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

    public string Content
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Content));
        }
    }

    public ChatsViewModel(MessengerState messengerState, ServiceSignalR signalR)
    {
        _messengerState = messengerState;
        _signalR = signalR;
    }

    public ICommand SendMessageCommand => new RelayCommand(async _ =>
    {
        if (SelectedChat is null)
            return;

        if (string.IsNullOrWhiteSpace(Content))
            return;

        Message message = new()
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = SelectedChat.Chat.Id,
            Content = Content.Trim(),
            IsDeleted = false,
            IsEdited = false,
            SentAt = DateTime.UtcNow
        };

        await _signalR.SendMessage(message);

        Content = null;
    });

    public ICommand RemoveMessageCommand => new RelayCommand(async p =>
    {
        if (p is not Message message)
            return;

        await _signalR.DeleteMessageAsync(message.Id, message.ChatId);
    });

    private async Task LoadMessagesAsync(ChatViewModel chat) =>
        await _signalR.GetMessagesAsync(chat.Chat.Id);

}
