using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace RussianNationalMessengerClient.Services;

public class ServiceSignalR : IAsyncDisposable
{
    public ObservableCollection<Chat> Chats = [];
    private static HubConnection? _connection;
    private readonly HttpClient _httpClient;

    public ServiceSignalR()
    {
        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };        
    }

    public static bool IsHubConnect()
    {
        if (_connection is null)
            return false;

        return _connection.State == HubConnectionState.Connected;
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

        _connection = await ConnectToHub(token);

        progress?.Report(100);
    }

    private async Task<HubConnection> ConnectToHub(string token)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7110/rnmhub", options =>
                options.AccessTokenProvider = () => Task.FromResult(token))
            .WithAutomaticReconnect()
            .Build();

        connection.On<Message>("ReceiveMessage", msg =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var chat = Chats.FirstOrDefault(x => x.Id == msg.ChatId);

                if (chat == null)
                    return;

                chat.Messages.Add(msg);
            });
        });

        connection.On<List<ChatMessagesDto>>("OnChatMessages", chatMessagesDto =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Chats.Clear();
                foreach (var item in chatMessagesDto)
                {
                    item.Chat.Messages = [.. item.Messages];
                    Chats.Add(item.Chat);
                }
            });
        });

        await connection.StartAsync();

        connection.Closed += Connection_Closed;
        return connection;
    }

    private async Task Connection_Closed(Exception? e)
    {
        MessageBox.Show($"соединение разорванно\n{e?.Message}\nState is {_connection?.State.ToString()}");
        await DisposeAsync();
    }

    public async Task SendMessage(Guid chatId, string message)
    {
        if (_connection is not null)
            await _connection.InvokeAsync("SendMessage", chatId, message);
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient.Dispose();

        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
