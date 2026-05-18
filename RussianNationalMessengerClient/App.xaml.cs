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

        MainWindow mainWindow = Services.GetRequiredService<MainWindow>();


        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // services

        services.AddSingleton<ServiceSignalR>();

        services.AddSingleton<NavigationService>();

        // viewmodels
        
        services.AddSingleton<MainViewModel>();

        services.AddTransient<AuthViewModel>();

        services.AddTransient<ChatsViewModel>();

        services.AddSingleton<MainWindow>();

        // views

        services.AddSingleton<MainWindow>();
    }
}
