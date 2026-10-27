using RussianNationalMessengerClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Services.ChatServer;

public partial class ServiceSignalR
{
    public event Action<List<Account>>? OnUsersFound;

    public async Task OnSearchUsers(List<Account> searchUsers) =>
        OnUsersFound?.Invoke(searchUsers);
}
