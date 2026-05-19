using Microsoft.Extensions.DependencyInjection;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System.Windows;

namespace RussianNationalMessengerClient;


public partial class App : Application
{
    //public static ObservableCollection<Chat> Chats { get; set; } = [];
    public static ServiceProvider Services { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ServiceCollection services = new();

        ConfigureServices(services);

        Services = services.BuildServiceProvider();

        NavigationService navigation = Services.GetRequiredService<NavigationService>();

        navigation.NavigateTo<AuthViewModel>();

        MainWindow mainWindow = Services.GetRequiredService<MainWindow>();

        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // services

        services.AddSingleton<ServiceSignalR>();

        services.AddSingleton<NavigationService>();


        // viewmodels

        services.AddTransient<AuthViewModel>();

        services.AddSingleton<MessengerState>();

        services.AddSingleton<MainWindow>();

        // views

        services.AddSingleton<MainWindow>();
    }
}
