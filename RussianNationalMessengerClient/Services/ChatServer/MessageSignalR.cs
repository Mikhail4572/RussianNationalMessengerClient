using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.ViewModels;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
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
                MessageId = msg.MessageId,
                SentAt = msg.SentAt
            };

            chat.Update(chat.Chat);

            chat.Messages.Add(new()
            {
                MessageId = msg.MessageId,
                Content = msg.Content,
                Author = msg.Author,
                ChatId = msg.ChatId,
                SentAt = msg.SentAt,
                Attachments = msg.Attachments?.Select(x => new UIMessageAttachment
                {
                    Id = x.Id,
                    ContentType = x.ContentType,
                    FileName = x.FileName,
                    Preview = null,
                    Size = x.Size
                }).OrderBy(x => x.ContentType).ToList()
            });
        });
    }

    public async Task OnRemoveMessage(string chatId, string messageId)
    {
        ChatViewModel? chat = _messengerState.GetChat(chatId);

        if (chat is null)
            return;

        var remove_message = chat.Messages.FirstOrDefault(m => m.MessageId == messageId);

        if (remove_message is null)
            return;

        App.Current.Dispatcher.Invoke(() =>
            chat.Messages.Remove(remove_message));

        if (chat.Chat.LastMessage is not null && remove_message.MessageId == chat.Chat.LastMessage.MessageId)
        {
            var last = chat.Messages.LastOrDefault();

            chat.Chat.LastMessage = last is null ? null : new()
            {
                MessageId = last.MessageId,
                Author = last.Author,
                Content = last.Content,
                SentAt = last.SentAt,
            };
        }

        chat.Update(chat.Chat);
    }

    public async Task GetMessagesAsync(string chatId)
    {
        List<Message> messages = await Connection.InvokeAsync<List<Message>>("GetMessages", chatId);

        ChatViewModel? chat = _messengerState.GetChat(chatId);

        if (chat is null)
            return;

        await chat.LoadMessages(messages);
    }
}
