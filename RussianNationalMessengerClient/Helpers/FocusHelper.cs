using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RussianNationalMessengerClient.Helpers;

public static class FocusHelper
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(FocusHelper),
            new UIPropertyMetadata(false));

    public static bool GetIsFocused(DependencyObject obj)
        => (bool)obj.GetValue(IsFocusedProperty);

    public static void SetIsFocused(DependencyObject obj, bool value)
        => obj.SetValue(IsFocusedProperty, value);
}
