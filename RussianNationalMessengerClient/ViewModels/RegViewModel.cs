using Microsoft.Win32;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Services.RNMApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class RegViewModel : ViewModelBase
{
    private readonly ApiRNM _client;
    private readonly NavigationService _navigation;

    public string Username
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public string Password
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public string Email
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public ICommand RegCommand => new RelayCommand(async _ =>
    {
        Username = Username?.Trim();
        Email = Email?.Trim();
        Password = Password?.Trim();

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
            return;

        try
        {
            await _client.Register(Username, Email, Password);

            _navigation.NavigateTo<VerifyRegViewModel>();
        }
        catch (Exception ex) 
        {
            MessageBox.Show(ex.Message);
        }
    });

    public ICommand ToAuthPageCommand => new RelayCommand(_ =>
        _navigation.NavigateTo<AuthViewModel>());

    public RegViewModel(ApiRNM client, NavigationService navigation)
    {
        _client = client;
        _navigation = navigation;
    }
}
