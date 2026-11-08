using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Services.WindowServices;

public class WindowService : IWindowService
{
    public void Show<TWindow>() where TWindow : Window
    {
        TWindow window = Activator.CreateInstance<TWindow>();
        window.Show();
    }

    public bool? ShowDialog<TWindow>() where TWindow : Window
    {
        TWindow window = Activator.CreateInstance<TWindow>();
        return window.ShowDialog();
    }
}
