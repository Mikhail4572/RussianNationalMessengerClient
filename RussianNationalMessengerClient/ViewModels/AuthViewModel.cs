using RussianNationalMessengerClient.Views.Windows;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.Classes;
using RussianNationalMessengerClient.Dtos;
using System.ComponentModel;
using System.Windows.Input;
using System.Net.Http;
using System.Windows;

namespace RussianNationalMessengerClient.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    // закрытие приложения
    public ICommand CloseCommand => new RelayCommand(_ => App.Current.Shutdown());

    public ICommand LoginCommand => new RelayCommand(async _ =>
    {
        try
        {
            Progress<int> progress = new(value => Value = value);
            ServiceSignalR service = new();
            await service.AuthorizationAsync(Login.UserName, Login.Password, progress);

            if (ServiceSignalR.IsHubConnect())
            {
                MainWindow mainWindow = new()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    DataContext = new MainViewModel()
                };

                mainWindow.Show();

                App.Current.Windows.OfType<AuthWindow>().FirstOrDefault()?.Close();
            }
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
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

    public LoginDto Login
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Login));
        }
    } = new();

    public AuthViewModel() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}