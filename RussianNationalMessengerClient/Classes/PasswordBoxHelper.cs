using System.Windows.Controls;
using System.Windows;

namespace RussianNationalMessengerClient.Classes;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged)
    );

    public static string GetPassword(DependencyObject obj)
        => (string)obj.GetValue(PasswordProperty);

    public static void SetPassword(DependencyObject obj, string value)
        => obj.SetValue(PasswordProperty, value);

    private static bool _updating;

    private static void OnPasswordChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (_updating) return;

        if (d is PasswordBox box)
        {
            box.PasswordChanged -= PasswordChanged;
            box.Password = e.NewValue?.ToString() ?? "";
            box.PasswordChanged += PasswordChanged;
        }
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
        _updating = true;
        if (sender is PasswordBox box)
            SetPassword(box, box.Password);

        _updating = false;
    }
}
