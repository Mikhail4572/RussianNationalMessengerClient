using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Services.RNMApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatsViewModel : ViewModelBase
{
    private readonly MessengerState _messengerState;
    private readonly ApiRNM _client;

    private readonly ServiceSignalR _signalR;

    private readonly AuthState _authState;
    public ObservableCollection<ChatViewModel> Chats => _messengerState.Chats;
    public ObservableCollection<Account> SearchUsers { get; set; } = [];

    public string SearchUserContent
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SearchUserContent));
        }
    }

    public Account? SelectedAccount
    {
        get => _messengerState.SelectedAccount;
        set
        {

            if (value is not null && _messengerState.Chats.FirstOrDefault(x => x.Chat.Members.Count == 2 && x.Chat.Members.Contains(value.Username))
                    is ChatViewModel chatVM)
            {
                SearchUserContent = null;
                SelectedChat = chatVM;
                return;
            }

            _messengerState.SelectedAccount = value;
            OnPropertyChanged(nameof(SelectedAccount));

            if (_messengerState.SelectedAccount is null)
                return;

            SearchUserContent = null;

            ChatViewModelFactory cvmf = new(_authState, _client);

            var chat = cvmf.Create(new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = null,
                IsGroup = false,
                CreatedAt = DateTime.UtcNow,
                LastMessage = null,
                IsCreated = false,
                Members =
               [
                   _authState.CurrentUser.Username,
                    _messengerState.SelectedAccount.Username
               ]
            });

            SelectedChat = chat;
        }
    }

    public ChatViewModel? SelectedChat
    {
        get => _messengerState.SelectedChat;
        set
        {
            _messengerState.SelectedChat = value;
            OnPropertyChanged(nameof(SelectedChat));

            if (value is not null && value.Chat.IsCreated)
            {
                Content = null;
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

    public Visibility VisibilityChatsLV => (!IsFocusedTbSearch) ? Visibility.Visible : Visibility.Collapsed;

    public bool IsFocusedTbSearch
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsFocusedTbSearch));
        }
    } = false;

    public ChatsViewModel(MessengerState messengerState, ServiceSignalR signalR, AuthState authState, ApiRNM client)
    {
        _authState = authState;
        _messengerState = messengerState;
        _signalR = signalR;

        SearchUsers.CollectionChanged += (s, e) =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    IsFocusedTbSearch = SearchUsers.Count > 0;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    IsFocusedTbSearch = SearchUsers.Count > 0;
                    break;

                case NotifyCollectionChangedAction.Reset:
                    IsFocusedTbSearch = false;
                    break;

                default: break;
            }
        };
    }

    public ICommand SearchUserCommand => new RelayCommand(async _ =>
    {
        if (string.IsNullOrEmpty(SearchUserContent))
        {
            SearchUsers.Clear();
            return;
        }

         var searchUsers = await _signalR.GetUsersByNameAsync(SearchUserContent);

        searchUsers ??= [];

        SearchUsers.Clear();

        // формируем список из тех контактов, чаты с которыми уже существуют
        List<Account> generalChatInSearch = [.. searchUsers.Where(x => Chats.Select(x => x.Chat).SelectMany(x => x.Members)
                    .Contains(x.Username) && x.Username != _authState.CurrentUser.Username)];

        // удаляем аккаунты из списка тех с кем чат уже есть
        foreach (var item in generalChatInSearch)
            searchUsers.Remove(item);

        // складываем списки таким образом, чтоб с начала были те с кем есть чат, а потом с кем нет
        searchUsers = [.. generalChatInSearch.Concat(searchUsers)];

        foreach (var item in searchUsers)
            SearchUsers.Add(item);

    });

    public ICommand DeleteChatCommand => new RelayCommand(async _ =>
    {
        if (SelectedChat is not null)
        {
            var mbres = MessageBox.Show("Вы точно хотите удалить чат со всей историей сообщений?\nЭто действие нельзя будет отменить.",
                    "Предупреждение.", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (mbres == MessageBoxResult.Yes)
                await _signalR.DeleteChatAsync(SelectedChat.Chat.Id);
        }
    });

    public ICommand RemoveMessageCommand => new RelayCommand(async p =>
    {
        if (p is not Message message)
            return;

        await _signalR.DeleteMessageAsync(message.MessageId, message.ChatId);
    });

    private async Task LoadMessagesAsync(ChatViewModel chat) =>
        await _signalR.GetMessagesAsync(chat.Chat.Id);

}
