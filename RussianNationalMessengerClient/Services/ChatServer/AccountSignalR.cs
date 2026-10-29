using RussianNationalMessengerClient.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
    public event Action<List<Account>>? OnUsersFound;

    public async Task OnSearchUsers(List<Account> searchUsers) =>
        OnUsersFound?.Invoke(searchUsers);


    public async Task GetUsersByNameAsync(string search) =>
        await Connection.InvokeAsync("GetUsersByName", search);
}
