using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Classes
{
    public class ChatViewModelFactory
    {
        private readonly AuthState _authState;

        public ChatViewModelFactory(AuthState authState)
        {
            _authState = authState;
        }

        public ChatViewModel Create(Chat chat) => new(chat, _authState);
    }
}
