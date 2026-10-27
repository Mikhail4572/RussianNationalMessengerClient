using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Views.Windows;
using RussianNationalMessengerClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Classes;
using System.Windows;

namespace RussianNationalMessengerClient;


public partial class App : Application
{
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
        // Singleton

        services.AddSingleton<ServiceSignalR>();

        services.AddSingleton<RequestToApiRNM>();

        services.AddSingleton<NavigationService>();

        services.AddSingleton<MessengerState>();

        services.AddSingleton<AuthState>();

        services.AddSingleton<ChatViewModelFactory>();

        services.AddSingleton<MainWindow>();

        // Transient

        services.AddTransient<AuthViewModel>();

        services.AddTransient<ChatsViewModel>();
    }
}
