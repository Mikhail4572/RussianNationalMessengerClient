using RussianNationalMessengerClient.Services.WindowServices;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Models;

namespace RussianNationalMessengerClient.Classes;

public class ChatViewModelFactory
{
    private readonly AuthState _authState;
    private readonly ApiRNM _client;
    private readonly IWindowService _windowService;
    

    public ChatViewModelFactory(AuthState authState, ApiRNM client, IWindowService windowService)
    {
        _windowService = windowService;
        _authState = authState;
        _client = client;
    }

    public ChatViewModel Create(Chat chat) => new(chat, _authState, _client, _windowService);
}
