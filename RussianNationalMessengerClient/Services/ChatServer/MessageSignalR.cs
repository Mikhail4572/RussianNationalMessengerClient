using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
    public async Task OnMessages(string chatId, List<Message> messages)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            ChatViewModel? chat = _messengerState.GetChat(chatId);

            if (chat is null)
                return;

            chat.LoadMessages(messages);
        });
    }

    public async Task OnMessage(Message msg)
    {
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
    }


    public async Task OnRemoveMessage(string chatId, string messageId)
    {
        ChatViewModel? chat = _messengerState.GetChat(chatId);

        if (chat is null)
            return;

        var remove_message = chat.Messages.FirstOrDefault(m => m.Id == messageId);

        if (remove_message is null)
            return;

        App.Current.Dispatcher.Invoke(() =>
            chat.Messages.Remove(remove_message));

        if (chat.Chat.LastMessage is not null && remove_message.Id == chat.Chat.LastMessage.MessageId)
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
    }

    public async Task SendMessage(Message message) =>
        await Connection.InvokeAsync("SendMessage", message);

    public async Task DeleteMessageAsync(string messageId, string chatId) =>
        await Connection.InvokeAsync("RemoveMessage", messageId, chatId);

    public async Task GetMessagesAsync(string chatId) =>
        await Connection.InvokeAsync("GetMessages", chatId);
}
