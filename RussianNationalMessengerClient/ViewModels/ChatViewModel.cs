using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.Services.WindowServices;
using RussianNationalMessengerClient.Views.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly AuthState _authState;
    private readonly ApiRNM _client;
    private readonly IWindowService _windowService;

    public Chat Chat { get; }

    public ObservableCollection<UIMessage> Messages { get; set; } = [];

    public ObservableCollection<UILoadAttachment> LoadAttachments { get; set; } = [];

    public bool IsSendButtonEnabled
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsSendButtonEnabled));
        }
    } = true;

    public UIMessage? SelectedMessage
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

    public async Task LoadMessages(List<Message> messages)
    {
        Messages.Clear();

        foreach (var message in messages)
        {
            Messages.Add(new()
            {
                MessageId = message.MessageId,
                Author = message.Author,
                ChatId = message.ChatId,
                Content = message.Content,
                SentAt = message.SentAt,
                Attachments = message.Attachments?.Select(x => new UIMessageAttachment
                {
                    Id = x.Id,
                    MessageId = message.MessageId,
                    ContentType = x.ContentType,
                    FileName = x.FileName,
                    Size = x.Size,
                    Preview = null
                }).ToList()
            });
        }
    }

    public ICommand PasteImageCommand => new RelayCommand(_ =>
    {
        if (Clipboard.ContainsText())
        {
            Content += Clipboard.GetText();
        }
    });

    public ICommand CopyChatCommand => new RelayCommand(_ =>
    {
        if (SelectedMessage is not null and not { Content: "" or null })
            Clipboard.SetText(SelectedMessage.Content);
    });

    public ICommand OpenAttachmentWindowCommand => new RelayCommand(async p =>
    {
        if (SelectedMessage is null || SelectedMessage.Attachments is null || SelectedMessage.Attachments.Count == 0) 
            return;

        _windowService.ShowDialog<AttachmentWindow>(SelectedMessage.Attachments);
    });

    public ICommand RemoveMessageCommand => new RelayCommand(async _ =>
    {
        if (SelectedMessage is not null)
        {
            var isComplete = await _client.DeleteMessage(SelectedMessage.MessageId);
            return;
        }
    });

    public ICommand SendMessageCommand => new RelayCommand(async _ =>
    {
        if (string.IsNullOrWhiteSpace(Content?.Trim()) && LoadAttachments.Count == 0)
            return;
    
        IsSendButtonEnabled = false;

        var result = await _client.SendMessage(Chat.Id, Content, LoadAttachments.Select(x => x.FullPath));

        IsSendButtonEnabled = true;

        LoadAttachments.Clear();
        Content = null;
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

    public ChatViewModel(Chat chat, AuthState authState, ApiRNM client, IWindowService windowService)
    {
        _windowService = windowService;
        Chat = chat;
        _authState = authState;
        _client = client;
        Messages.CollectionChanged += async (s, e) =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems.Count > 0 && e.NewItems[0] is UIMessage add && add.Attachments is not null && add.Attachments.Any(x => x.IsImage))
                    {
                        var attachments = await Task.WhenAll(add.Attachments.Select(async x => new UIMessageAttachment
                        {
                            Id = x.Id,
                            MessageId = add.MessageId,
                            ContentType = x.ContentType,
                            FileName = x.FileName,
                            Size = x.Size,
                            Preview = x.ContentType.StartsWith("image/") ? ConvertImage.ConvertBytesToBitmapImage(await _client.DownloadFile(add.MessageId, x.Id)) : null
                        }) ?? []);

                        foreach (var item in add.Attachments.Where(x => x.IsImage))
                            item.Preview = attachments.FirstOrDefault(x => x.Id == item.Id)?.Preview;
                    }

                break;
            }
        };
    }
}
