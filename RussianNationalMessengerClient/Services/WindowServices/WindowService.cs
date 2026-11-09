using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Services.WindowServices;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Show<TWindow>(object parameter) where TWindow : Window
    {
        TWindow window = _serviceProvider.GetRequiredService<TWindow>();

        if (window.DataContext is IParameterReceiver receiver)
        {
            receiver.Initialize(parameter);
        }

        window.Show();
    }

    public bool? ShowDialog<TWindow>(object parameter) where TWindow : Window
    {
        TWindow window = _serviceProvider.GetRequiredService<TWindow>();

        if (window.DataContext is IParameterReceiver receiver)
        {
            receiver.Initialize(parameter);
        }

        return window.ShowDialog();
    }
}