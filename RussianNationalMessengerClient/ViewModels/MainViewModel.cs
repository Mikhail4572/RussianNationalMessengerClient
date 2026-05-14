using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using RussianNationalMessengerClient.Classes;

namespace RussianNationalMessengerClient.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Chat> Chats { get; set; }

    public Page FrChatContent
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(FrChatContent));
        }
    }

    public ICommand ShowDialogCommand => new RelayCommand(_ =>
    {

    });

    public MainViewModel() 
    {
        Chats = [.. ServiceSignalR.SSR!.Chats];
        ServiceSignalR.SSR.Chats.CollectionChanged += ChatMembers_CollectionChanged;
    }

    private void ChatMembers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null && e.NewItems.Count > 0 && e.NewItems[0] is Chat addChat)
                {
                    Chats.Add(addChat);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is not null && e.OldItems.Count > 0 && e.OldItems[0] is Chat removeChat)
                {
                    Chats.Remove(removeChat);
                }
                break;

            default:
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
