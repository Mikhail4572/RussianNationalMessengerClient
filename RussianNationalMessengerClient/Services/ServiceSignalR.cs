using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
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

    public async Task GetMessagesAsync(string chatId) =>
        await Connection.SendAsync("GetMessages", chatId);

    public async Task GetChatsAsync() =>
        await Connection.SendAsync("GetChats");

    private async Task Connection_Closed(Exception? e) =>
        MessageBox.Show($"соединение разорванно\n{e?.Message}\nState is {Connection.State}");

    public async Task SendMessage(Message message)
    {
        try
        {
            await Connection.InvokeAsync("SendMessage", message);
        }
        catch { }
    }
}
