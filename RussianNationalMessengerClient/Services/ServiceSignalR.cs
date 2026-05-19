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

    private readonly HttpClient _httpClient;

    //на получение списка сообщений
    public event Action<List<Message>>? OnMessages;

    //на получение сообщения
    public event Action<Message>? OnMessage;

    public ServiceSignalR(MessengerState messengerState)
    {
        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        _messengerState = messengerState;
    }

    private async Task<string> Login(string username, string password)
    {
        LoginDto login = new()
        {
            UserName = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7110/api/Login/login", login);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(await response.Content.ReadAsStringAsync());

        var json = await response.Content.ReadAsStringAsync();

        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(json) ??
            throw new ArgumentNullException(nameof(json) + " is null");

        return loginResponse.Token;
    }

    public async Task AuthorizationAsync(string username, string password, IProgress<int>? progress = null)
    {
        progress?.Report(10);
        var token = await Login(username, password);

        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token) + " is null or empty");

        progress?.Report(40);

        Connection = await ConnectToHub(token);

        progress?.Report(100);
    }

    private async Task<HubConnection> ConnectToHub(string token)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7110/rnmhub", options =>
                options.AccessTokenProvider = () => Task.FromResult(token))
            .WithAutomaticReconnect()
            .Build();

        connection.On<List<Message>>("onMessages", messages =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                OnMessages?.Invoke(messages);

            });
        });

        connection.On<Message>("onMessage", msg =>
        {
            // добавить изменение LastMessage в Chat
            App.Current.Dispatcher.Invoke(() =>
            {
                //OnMessage?.Invoke(msg)
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

    public async Task GetChatsAsync() =>
        await Connection.SendAsync("GetChats");

    private async Task Connection_Closed(Exception? e) =>
        MessageBox.Show($"соединение разорванно\n{e?.Message}\nState is {Connection?.State.ToString()}");

    public async Task SendMessage(string chatId, string message)
    {
        if (Connection is not null)
            await Connection.InvokeAsync("SendMessage", chatId, message);
    }
}
