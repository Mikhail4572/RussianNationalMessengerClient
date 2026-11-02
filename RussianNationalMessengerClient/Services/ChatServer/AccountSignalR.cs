using RussianNationalMessengerClient.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
    public async Task<List<Account>> GetUsersByNameAsync(string search) =>
        await Connection.InvokeAsync<List<Account>>("GetUsersByName", search);
}
