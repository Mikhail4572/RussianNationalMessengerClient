using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Services.RNMApi;
using System.Net.Http;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class AuthViewModel : ViewModelBase
{
    public ICommand LoginCommand => new RelayCommand(async _ =>
    {
        IProgress<int> progress = new Progress<int>(value => Value = value);

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(UserName, nameof(UserName));
            ArgumentException.ThrowIfNullOrWhiteSpace(Password, nameof(Password));

            var loginResult = await _client.Login(UserName, Password);

            if (loginResult.loginResponseDto is null)
            {
                ErrorMessage = loginResult.problem is null ? "Ошибка, попробуйте позже("
                    : "Title: " + loginResult.problem.Title;

                return;
            }

            progress.Report(10);

            var response = await _service.AuthorizationAsync(loginResult.loginResponseDto, progress);

            _authState.Token = response.Token;

            _authState.CurrentUser = response.User;

            await _service.Connection.InvokeAsync("GetChats");

            progress.Report(100);

            if (_service.Connection.State == HubConnectionState.Connected)
                _navigation.NavigateTo<ChatsViewModel>();
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            Value = 0;
        }
    },
        _ => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password));

    public string UserName
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(UserName));
        }
    } = "mikhail";

    public string Password
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Password));
        }
    } = "1111";

    public int Value
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Value));
        }
    } = 0;

    public string ErrorMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    private readonly AuthState _authState;
    private readonly ServiceSignalR _service;
    private readonly NavigationService _navigation;
    private readonly ApiRNM _client;

    public AuthViewModel(ServiceSignalR service, NavigationService navigation,
        AuthState authState, ApiRNM client)
    {
        _client = client;
        _service = service;
        _navigation = navigation;
        _authState = authState;
    }
}