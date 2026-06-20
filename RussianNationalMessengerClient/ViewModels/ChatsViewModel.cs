using RussianNationalMessengerClient.Classes;
using System.Collections.Specialized;
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
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedAccount));

            if (field is null)
                return;

            SearchUserContent = null;

            ChatViewModelFactory cvmf = new(_authState);
            SelectedChat = cvmf.Create(new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = null,
                IsGroup = false,
                CreatedAt = DateTime.Now,
                LastMessage = null,
                IsCreated = false,
                Members =
                [
                    _authState.CurrentUser.Id,
                    field.Id
                ]
            });
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

    public ChatsViewModel(MessengerState messengerState, ServiceSignalR signalR, AuthState authState)
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

        _signalR.OnUsersFound += searchUsers =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                SearchUsers.Clear();

                // формируем список из тех контактов, чаты с которыми уже существуют
                List<Account> generalChatInSearch = [.. searchUsers.Where(x => Chats.Select(x => x.Chat).SelectMany(x => x.Members).Contains(x.Id) && x.Id != _authState.CurrentUser.Id)];

                // удаляем аккаунты из списка тех с кем чат уже есть
                foreach (var item in generalChatInSearch)
                    searchUsers.Remove(item);

                // складываем списки таким образом, чтоб с начала были те с кем есть чат, а потом с кем нет
                searchUsers = [.. generalChatInSearch.Concat(searchUsers)];

                foreach (var item in searchUsers)
                    SearchUsers.Add(item);
            });
        };
    }

    public ICommand SearchUserCommand => new RelayCommand(async _ =>
    {
        if (string.IsNullOrEmpty(SearchUserContent))
        {
            SearchUsers.Clear();
            return;
        }

        await _signalR.GetUsersByNameAsync(SearchUserContent);
    });

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

        if (!SelectedChat.Chat.IsCreated)
        {
            // создаём чат
            await _signalR.CreateChatAsync(message, null, [.. SelectedChat.Chat.Members]);
        }


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
