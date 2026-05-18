using RussianNationalMessengerClient.Views.Windows;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Classes;
using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using System.ComponentModel;
using System.Windows.Input;
using System.Net.Http;
using System.Windows;

namespace RussianNationalMessengerClient.ViewModels;

public class AuthViewModel : ViewModelBase
{
    // закрытие приложения
    public ICommand CloseCommand => new RelayCommand(_ => App.Current.Shutdown());

    public ICommand LoginCommand => new RelayCommand(async _ =>
    {
        Progress<int> progress = new(value => Value = value);
        try
        {
            await _service.AuthorizationAsync(Login.UserName, Login.Password, progress);

            if (_service.Connection.State == HubConnectionState.Connected)
                _navigation.NavigateTo<ChatsViewModel>();
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            Value = 0;
        }
    },
        _ => !string.IsNullOrWhiteSpace(Login.UserName) && !string.IsNullOrWhiteSpace(Login.Password));

    public int Value
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Value));
        }
    } = 0;

    public LoginDto Login
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Login));
        }
    } = new();

    private readonly ServiceSignalR _service;
    private readonly NavigationService _navigation;

    public AuthViewModel(ServiceSignalR service, NavigationService navigation)
    {
        _service = service;
        _navigation = navigation;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
