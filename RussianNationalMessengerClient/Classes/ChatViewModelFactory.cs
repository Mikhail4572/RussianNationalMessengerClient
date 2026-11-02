using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Classes;

public class ChatViewModelFactory
{
    private readonly AuthState _authState;
    private readonly ApiRNM _client;

    public ChatViewModelFactory(AuthState authState, ApiRNM client)
    {
        _authState = authState;
        _client = client;
    }

    public ChatViewModel Create(Chat chat) => new(chat, _authState, _client);
}
