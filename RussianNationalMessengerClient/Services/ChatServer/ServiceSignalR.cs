using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
    public HubConnection Connection { get; private set; }

    private readonly MessengerState _messengerState;   

    public ServiceSignalR(MessengerState messengerState) =>
        _messengerState = messengerState;

    public async Task<LoginResponseDto> AuthorizationAsync(LoginResponseDto loginResponse, IProgress<int>? progress = null)
    {
        if (string.IsNullOrEmpty(loginResponse.Token))
            throw new ArgumentNullException(nameof(loginResponse.Token) + " is null or empty");

        progress?.Report(10);

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

        // Чаты
        connection.On<string>("onDeleteChat", OnDeleteChat);

        connection.On<List<Chat>>("onChats", OnChats);

        connection.On<Chat, Message>("onCreateChat", OnCreateChat);


        // Пользователи
        connection.On<List<Account>>("onSearchUsers", OnSearchUsers);


        // Сообщения
        connection.On<string, string>("onRemoveMessage", OnRemoveMessage);

        connection.On<string, List<Message>>("onMessages", OnMessages);

        connection.On<Message>("onMessage", OnMessage);


        await connection.StartAsync();

        connection.Closed += Connection_Closed;
        return connection;
    }

    private async Task Connection_Closed(Exception? e) =>
       MessageBox.Show($"соединение разорванно\n{e?.Message}\nState is {Connection.State}");
}

