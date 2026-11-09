using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RussianNationalMessengerClient.ViewModels;

public class AttachmentWindowViewModel : ViewModelBase, IParameterReceiver
{
    private readonly ApiRNM _client;
    private ObservableCollection<UIMessageAttachment> _attachments { get; set; } = [];
    
    private int _currentIndex = 0;

    public AttachmentWindowViewModel(ApiRNM client) =>
        _client = client;

    public UIMessageAttachment Attachment
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Attachment));
        }
    }

    public Visibility ContentInfoVisibility => Attachment?.IsImage == true ? Visibility.Collapsed : Visibility.Visible;

    public ICommand SaveCommand => new RelayCommand(async _ =>
    {
        byte[]? bytes = null;

        if (Attachment.Preview is not null)
        {
            bytes = ConvertImage.BitmapImageToBytes(Attachment.Preview, Path.GetExtension(Attachment.FileName));
        }
        else
            bytes = await _client.DownloadFile(Attachment.MessageId, Attachment.Id);

        if (bytes is not null && bytes.Length > 0)
        {
            SaveFileDialog sfd = new()
            {
                FileName = Attachment.FileName,
                Filter = "Все файлы (*.*)|*.*"
            };

            if (sfd.ShowDialog() == true)
                await File.WriteAllBytesAsync(sfd.FileName, bytes);

        }
        
    });

    public ICommand NextCommand => new RelayCommand(_ =>
    {
        try
        {
            _currentIndex++;
            Attachment = _attachments[_currentIndex];
        }
        catch
        {
            Attachment = _attachments[0];
            _currentIndex = 0;
        }
    });

    public ICommand LastCommand => new RelayCommand(_ =>
    {
        try
        {
            _currentIndex--;
            Attachment = _attachments[_currentIndex];
        }
        catch
        {
            Attachment = _attachments[^1];
            _currentIndex = _attachments.Count - 1;
        }
    });

    public void Initialize(object parameter)
    {
        var attachments = (parameter as IEnumerable<UIMessageAttachment>) ?? [];
        _attachments.Clear();

        foreach (var item in attachments)
            _attachments.Add(item);

        Attachment = attachments.FirstOrDefault();
    }
}
