using Microsoft.AspNetCore.SignalR.Client;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Dtos;
using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Services.RNMApi;
using System.Windows;
using System.Windows.Input;

namespace RussianNationalMessengerClient.ViewModels;

public class VerifyRegViewModel : ViewModelBase
{
    public string Code
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    public ICommand VerifyCodeCommand => new RelayCommand(async _ =>
    {
        Code = Code?.Trim();

        try
        {
            if (string.IsNullOrWhiteSpace(Code))
                return;

            var dto = await _client.VerifyRegistration(Code);

            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            await _service.AuthorizationAsync(dto);

            await _service.GetChatsAsync();

            _authState.Token = dto.Token;
            _authState.CurrentUser = dto.User;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    });

    private readonly ApiRNM _client;
    private readonly AuthState _authState;
    private readonly ServiceSignalR _service;

    public VerifyRegViewModel(ApiRNM client, AuthState authState, ServiceSignalR service)
    {
        _client = client;
        _authState = authState;
        _service = service;
    }
}
