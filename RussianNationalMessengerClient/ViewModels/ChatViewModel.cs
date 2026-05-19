using RussianNationalMessengerClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatViewModel : ViewModelBase
{
    public Chat Chat { get; }

    public ObservableCollection<Message> Messages { get; set; } = [];

    public string DisplayName => Chat.Name ?? string.Empty;

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

            return $"{Chat.LastMessage.Author}:";
        }
    }

    public ChatViewModel(Chat chat) => 
        Chat = chat;
}
