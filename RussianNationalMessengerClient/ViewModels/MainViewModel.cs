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

public class MainViewModel : ViewModelBase
{
    public ViewModelBase CurrentViewModel
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

 
    public MainViewModel(AuthViewModel authViewModel) 
    {
        CurrentViewModel = authViewModel;
    }
}
