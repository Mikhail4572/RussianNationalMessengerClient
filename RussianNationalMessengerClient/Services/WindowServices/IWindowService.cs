using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Services.WindowServices;

public interface IWindowService
{
    void Show<TWindow>() where TWindow : Window;
    bool? ShowDialog<TWindow>() where TWindow : Window;
}
