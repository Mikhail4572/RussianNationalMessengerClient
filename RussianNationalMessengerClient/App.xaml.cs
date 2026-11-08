using Microsoft.Extensions.DependencyInjection;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Services.ChatServer;
using RussianNationalMessengerClient.Services.RNMApi;
using RussianNationalMessengerClient.Services.WindowServices;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
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
        
        services.AddSingleton<ApiRNM>();

        services.AddSingleton<NavigationService>();

        services.AddSingleton<MessengerState>();

        services.AddSingleton<AuthState>();

        services.AddSingleton<ChatViewModelFactory>();

        services.AddSingleton<MainWindow>();

      //  services.AddSingleton<IWindowService, WindowService>();

        // Transient

        services.AddTransient<AuthViewModel>();

        services.AddTransient<ChatsViewModel>();
    }
}
