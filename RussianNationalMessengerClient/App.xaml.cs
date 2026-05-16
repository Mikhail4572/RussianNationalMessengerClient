using RussianNationalMessengerClient.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RussianNationalMessengerClient;


public partial class App : Application
{
    public static ServiceSignalR CurrentConnectToSSR { get; set; }
}
