using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Services.RNMApi;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly AuthState _authState;
    private readonly ApiRNM _client;

    public Chat Chat { get; }

    public ObservableCollection<Message> Messages { get; set; } = [];

    public ObservableCollection<UILoadAttachment> LoadAttachments { get; set; } = [];

    public Message? SelectedMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedMessage));
        } 
    }

    public string DisplayName
    {
        get
        {
            if (Chat is null)
                return "Chat is null";

            if (Chat.IsGroup)
                return Chat.Name ?? "No name group";

            var interlocutor = Chat.Members.FirstOrDefault(x => x != _authState.CurrentUser.Username, string.Empty);

            return interlocutor;
        }
    }

    public string LastMessageContent
    {
        get
        {
            if (Chat.LastMessage is null)
                return string.Empty;

            return Chat.LastMessage.Content ?? string.Empty;
        }
    }

    public string LastMessageAuthor
    {
        get
        {
            if (Chat.LastMessage is null)
                return string.Empty;

            if (string.IsNullOrEmpty(Chat.Name))
                return Chat.LastMessage.Author == _authState.CurrentUser.Username ? "Вы:" : Chat.LastMessage.Author;

            return string.Empty;
        }
    }

    public string Content
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Content));
        }
    }

    public void Update(Chat chat)
    {
        chat.LastMessage = chat.LastMessage;

        chat.Name = chat.Name;

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(LastMessageContent));
        OnPropertyChanged(nameof(LastMessageAuthor));
    }

    public void LoadMessages(List<Message> messages)
    {
        Messages.Clear();

        foreach (var message in messages)
            Messages.Add(message);
    }

    public ICommand PasteContentCommand => new RelayCommand(_ =>
    {
        if (Clipboard.ContainsText())
            Content += Clipboard.GetText();
    });

    public ICommand CopyChatCommand => new RelayCommand(_ =>
    {
        if (SelectedMessage is not null and not { Content: "" or null })
            Clipboard.SetText(SelectedMessage.Content);
    });

    public ICommand RemoveMessageCommand => new RelayCommand(async _ =>
    {
        //throw new NotImplementedException();
        if (SelectedMessage is not null)
        {
            var isComplete = await _client.DeleteMessage(SelectedMessage.MessageId);
            Console.WriteLine();
        }
    });

    public ICommand SendMessageCommand => new RelayCommand(async _ =>
    {
        if (string.IsNullOrWhiteSpace(Content?.Trim()) && LoadAttachments.Count == 0)
            return;

        Message addMessage = new()
        {
            MessageId = "1",
            Author = _authState.CurrentUser.Username,
            ChatId = Chat.Id,
            Content = Content,
            IsSend = null,
            SentAt = DateTime.UtcNow,
            UILoadAttachments = LoadAttachments.ToList()
        };

       // Messages.Add(addMessage);

        //var result = 
        await _client.SendMessage(Chat.Id, Content, LoadAttachments.Select(x => x.FullPath));

        //if (result.problem is not null)
        //{
        //    MessageBox.Show($"Ошибка - {result.problem.Title}");
        //    Messages.Remove(addMessage);
        //    addMessage.IsSend = false;
        //    return;
        //}

        //if(result.returnMessage is null)
        //{
        //    MessageBox.Show($"Неизвестная ошибка");
        //    Messages.Remove(addMessage);
        //    return;
        //}

        //var index = Messages.IndexOf(addMessage);

        //if (index != -1)
        //{
        //    result.returnMessage.UILoadAttachments = addMessage.UILoadAttachments;
            
        //    Messages.Remove(addMessage);

        //    Messages.Insert(index, result.returnMessage);
        //}
        
        LoadAttachments.Clear();
        Content ??= null;
    });

    public ICommand AddAttachment => new RelayCommand(async _ =>
    {
        if (LoadAttachments.Count == 5)
        {
            MessageBox.Show("не более 5 вложений");
            return;
        }

        OpenFileDialog ofd = new()
        {
            Multiselect = true
        };

        if (ofd.ShowDialog() == true)
        {

            if ((ofd.FileNames.Length + LoadAttachments.Count) > 5)
                return;

            string[] imageEx = [".jpg", ".png"];

            foreach (var item in ofd.FileNames)
            {
                LoadAttachments.Add(new()
                {
                    FullPath = item,
                    ImageSource = imageEx.Contains(Path.GetExtension(item.ToLower())) ? ConvertImage.GetImageSourceFromPath(item) : null,
                    Stream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read)
                });
            }
        }
    });

    public ChatViewModel(Chat chat, AuthState authState, ApiRNM client)
    {
        Chat = chat;
        _authState = authState;
        _client = client;
    }
}
