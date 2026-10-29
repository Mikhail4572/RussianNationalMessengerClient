using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Classes;
using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Dtos;
using System.Windows.Input;
using System.Net.Http;

namespace RussianNationalMessengerClient.ViewModels;

public class AuthViewModel : ViewModelBase
{ 
    public ICommand LoginCommand => new RelayCommand(async _ =>
    {
        IProgress<int> progress = new Progress<int>(value => Value = value);

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Login.UserName, nameof(Login.UserName));
            ArgumentException.ThrowIfNullOrWhiteSpace(Login.Password, nameof(Login.Password));

            var loginResult = await _client.Login(Login.UserName, Login.Password);

            if(loginResult.loginResponseDto is null)
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

    public string ErrorMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public LoginDto Login
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Login));
        }
    } = new();

    private readonly AuthState _authState;
    private readonly ServiceSignalR _service;
    private readonly NavigationService _navigation;
    private readonly RequestToApiRNM _client;

    public AuthViewModel(
        ServiceSignalR service, NavigationService navigation, 
        AuthState authState, RequestToApiRNM client)
    {
        _client = client;
        _service = service;
        _navigation = navigation;
        _authState = authState;
    }
}
