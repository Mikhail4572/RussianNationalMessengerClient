using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RussianNationalMessengerClient.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{

    public ViewModelBase() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
