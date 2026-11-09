using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Services.WindowServices;

public interface IWindowService
{
    void Show<TWindow>(object parameter) where TWindow : Window;
    bool? ShowDialog<TWindow>(object parameter) where TWindow : Window;
}