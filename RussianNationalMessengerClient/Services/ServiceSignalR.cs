using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace RussianNationalMessengerClient.Services;

public class ServiceSignalR
{
    public HubConnection Connection { get; private set; }

    private readonly MessengerState _messengerState;

    public event Action<List<Account>>? OnUsersFound;

    public ServiceSignalR(MessengerState messengerState) =>
        _messengerState = messengerState;

    public async Task<LoginResponseDto> AuthorizationAsync(string username, string password, IProgress<int>? progress = null)
    {
        progress?.Report(10);
        var loginResponse = await RequestToApiRNM.Login(username, password);

        if (string.IsNullOrEmpty(loginResponse.Token))
            throw new ArgumentNullException(nameof(loginResponse.Token) + " is null or empty");

        progress?.Report(40);

        Connection = await ConnectToHub(loginResponse.Token);

        progress?.Report(100);

        return loginResponse;

    }

    private async Task<HubConnection> ConnectToHub(string token)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7110/rnmhub", options =>
                options.AccessTokenProvider = () => Task.FromResult(token))
            .WithAutomaticReconnect()
            .Build();

        connection.On<string>("onDeleteChat", chatId =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var remove_chat = _messengerState.Chats.FirstOrDefault(x => x.Chat.Id == chatId);

                if (remove_chat is not null)
                    _messengerState.Chats.Remove(remove_chat);

                _messengerState.RemoveChat(chatId);
            });
        });


        connection.On<string, Chat, Message>("onCreateChat", (oldChatId, chat, firstMessage) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _messengerState.LoadChats([chat]);

                var chatVM = _messengerState.GetChat(chat.Id);

                chatVM?.Messages.Add(firstMessage);

                var dc = (App.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.DataContext as 
                    NavigationService)?.CurrentViewModel as ChatsViewModel;

                dc?.SelectedChat = chatVM;

                //_messengerState.SelectedChat = null;
                //_messengerState.SelectedAccount = null;
                //_messengerState.SelectedChat = chatVM;

                MessageBox.Show("");
            });
        });

        connection.On<List<Account>>("onSearchUsers", searchUsers =>
            OnUsersFound?.Invoke(searchUsers));

        connection.On<string, string>("onRemoveMessage", (chatId, messageId) =>
        {
            ChatViewModel? chat = _messengerState.GetChat(chatId);

            if (chat is null)
                return;

            var remove_message = chat.Messages.FirstOrDefault(m => m.Id == messageId);

            if (remove_message is null)
                return;

            App.Current.Dispatcher.Invoke(() =>
                chat.Messages.Remove(remove_message));

            if(chat.Chat.LastMessage is not null && remove_message.Id == chat.Chat.LastMessage.MessageId)
            {
                var last = chat.Messages.LastOrDefault();

                chat.Chat.LastMessage = last is null ? null : new()
                {
                    MessageId = last.Id,
                    Author = last.Author,
                    Content = last.Content,
                    SentAt = last.SentAt,
                };
            }

            chat.Update(chat.Chat);
        });

        connection.On<string, List<Message>>("onMessages", (chatId, messages) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatViewModel? chat = _messengerState.GetChat(chatId);

                if (chat is null)
                    return;

                chat.LoadMessages(messages);
            });
        });

        connection.On<Message>("onMessage", msg =>
        {
            // добавить изменение LastMessage в Chat
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatViewModel? chat = _messengerState.GetChat(msg.ChatId);

                if (chat is null)
                    return;

                chat.Chat.LastMessage = new()
                {
                    Author = msg.Author,
                    Content = msg.Content,
                    MessageId = msg.Id,
                    SentAt = msg.SentAt
                };

                chat.Update(chat.Chat);

                chat.Messages.Add(msg);
            });
        });

        connection.On<List<Chat>>("onChats", chats =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _messengerState.LoadChats(chats);
            });
        });

        await connection.StartAsync();

        connection.Closed += Connection_Closed;
        return connection;
    }

    public async Task DeleteChatAsync(string chatId) =>
        await Connection.SendAsync("DeleteChat", chatId);

    public async Task CreateChatAsync(Message firstMessage, string groupName, string[] members) =>
        await Connection.SendAsync("CreateChat", firstMessage, groupName, members);

    public async Task GetUsersByNameAsync(string name) =>
        await Connection.SendAsync("GetUsersByName", name);

    public async Task GetMessagesAsync(string chatId) =>
        await Connection.SendAsync("GetMessages", chatId);

    public async Task DeleteMessageAsync(string messageId, string chatId) =>
        await Connection.SendAsync("RemoveMessage", messageId, chatId);

    public async Task GetChatsAsync() =>
        await Connection.SendAsync("GetChats");

    private async Task Connection_Closed(Exception? e) =>
        MessageBox.Show($"соединение разорванно\n{e?.Message}\nState is {Connection.State}");

    public async Task SendMessage(Message message)
    {
        await Connection.InvokeAsync("SendMessage", message);
    }
}
