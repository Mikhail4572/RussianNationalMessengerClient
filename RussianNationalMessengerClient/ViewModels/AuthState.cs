using System;
using System.Collections.Generic;
using System.Text;
using RussianNationalMessengerClient.Models;

namespace RussianNationalMessengerClient.ViewModels;

public class AuthState : ViewModelBase
{
    public Account CurrentUser
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentUser));
        }
    }

    public string? Token
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Token));
        }
    }

    public bool IsAuthorized => !string.IsNullOrWhiteSpace(Token);
}
