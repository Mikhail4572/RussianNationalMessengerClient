using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services.RNMApi;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatViewModel : ViewModelBase
{

    private readonly AuthState _authState;
    private readonly ApiRNM _client;

    public Chat Chat { get; }

    public ObservableCollection<Message> Messages { get; set; } = [];

    public ObservableCollection<string> FilePaths { get; set; } = [];

    public string DisplayName
    {
        get
        {
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

    public ICommand SendMessageCommand => new RelayCommand(async _ =>
    {
        if (string.IsNullOrWhiteSpace(Content) && FilePaths.Count == 0)
            return;

        var result = await _client.SendMessage(Chat.Id, Content, FilePaths);

        if (result.problem is not null)
        {
            MessageBox.Show($"Ошибка - {result.problem.Title}");
            return;
        }

        FilePaths.Clear();
        Content = null;
    });

    public ICommand AddAttachment => new RelayCommand(async _ =>
    {
        if (FilePaths.Count == 5)
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
            if ((ofd.FileNames.Length + FilePaths.Count) > 5)
                return;

            foreach (var item in ofd.FileNames)
                FilePaths.Add(item);

        }

    });

    public ChatViewModel() { }

    public ChatViewModel(Chat chat, AuthState authState, ApiRNM client)
    {
        Chat = chat;
        _authState = authState;
        _client = client;
    }
}
