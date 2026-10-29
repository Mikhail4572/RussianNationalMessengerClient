using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System;
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

    public async Task OnCreateChat(Chat chat, Message firstMessage)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            _messengerState.LoadChats([chat]);

            var chatVM = _messengerState.GetChat(chat.Id);

            chatVM?.Messages.Add(firstMessage);

            var vm = (App.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.DataContext as
                NavigationService)?.CurrentViewModel as ChatsViewModel;

            vm?.SelectedAccount = null;
            vm?.SelectedChat = chatVM;
        });
    }

    public async Task CreateChatAsync(Message message, string groupName, string[]members) =>
        await Connection.InvokeAsync("CreateChat", message, groupName, members);

    public async Task DeleteChatAsync(string chatId) =>
        await Connection.InvokeAsync("DeleteChat", chatId);
}
