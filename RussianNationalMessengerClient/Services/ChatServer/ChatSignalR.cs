using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Services.ChatServer;


public partial class ServiceSignalR
{
    public async Task OnChats(List<Chat> chats)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            _messengerState.LoadChats(chats);
        });
    }

    public async Task OnDeleteChat(string chatId)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            var remove_chat = _messengerState.Chats.FirstOrDefault(x => x.Chat.Id == chatId);

            if (remove_chat is not null)
                _messengerState.Chats.Remove(remove_chat);

            _messengerState.RemoveChat(chatId);
        });
    }

    public async Task<Chat?> CreateChatAsync(string groupName, string[] members) =>    
        await Connection.InvokeAsync<Chat?>("CreateChat", groupName, members);

    public async Task OnCreateChat(Chat chat)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            _messengerState.LoadChats([chat]);
        });
    }
    public async Task DeleteChatAsync(string chatId) =>
        await Connection.SendAsync("DeleteChat", chatId);

}
