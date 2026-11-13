using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Models.UIModels;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.Services.WindowServices;
using RussianNationalMessengerClient.Views.Windows;
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
    private ObservableCollection<UIMessageAttachment> _attachments = [];
    
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

    public ICommand SaveCommand => new RelayCommand(async _ =>
    {
        byte[]? bytes = null;

        if (Attachment.Preview is not null)
            bytes = ConvertImage.BitmapImageToBytes(Attachment.Preview, Path.GetExtension(Attachment.FileName));

        else
            bytes = await _client.DownloadFile(Attachment.Id);

        if (bytes is not null && bytes.Length > 0)
        {
            var extension = Path.GetExtension(Attachment.FileName);

            SaveFileDialog sfd = new()
            {
                FileName = Attachment.FileName,
                Filter = $"Фото (*{extension})|*{extension}|Все файлы (*.*)|*.*"
            };

            if (sfd.ShowDialog() == true)
                await File.WriteAllBytesAsync(sfd.FileName, bytes);
        }        
    });

    public ICommand CloseWindowCommand => new RelayCommand(_ => 
    {
        var window = App.Current.Windows.OfType<AttachmentWindow>().FirstOrDefault(x => Object.ReferenceEquals(x.DataContext, this));
        window?.Close();
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
    },
        _ => _attachments.Count != 1);

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
    },
        _ => _attachments.Count != 1);


    public ICommand CopyImageCommand => new RelayCommand(_ =>
    {
        if (Attachment.Preview is null)
            return;

        Clipboard.SetImage(Attachment.Preview);
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
