using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RussianNationalMessengerClient.ViewModels;

public class DialogViewModel : INotifyPropertyChanged
{

    public DialogViewModel() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
