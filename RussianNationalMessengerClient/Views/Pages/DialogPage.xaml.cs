using RussianNationalMessengerClient.Models;
using RussianNationalMessengerClient.Services;
using RussianNationalMessengerClient.ViewModels;
using RussianNationalMessengerClient.Views.Windows;
using System.Windows;
using System.Windows.Controls;

namespace RussianNationalMessengerClient.Views.Pages;

public partial class DialogPage : UserControl
{
    public DialogPage()
    {
        InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (DataContext is ChatViewModel vm)
            {
                vm.Messages.CollectionChanged += (s, e) =>
                {
                    if (LvMessages.Items.Count > 0)
                        LvMessages.ScrollIntoView(LvMessages.Items[^1]);
                };
            }
        };
    }
    private void MenuItemCopyMessage_Click(object sender, RoutedEventArgs e) 
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is not Message message)
            return;

        Clipboard.SetText(message.Content);
    }
    private void MenuItemRemoveMessage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;

        if (menuItem.DataContext is not Message message)
            return;

        if (App.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.DataContext is not NavigationService dataContext)
            return;

        (dataContext.CurrentViewModel as ChatsViewModel)?.RemoveMessageCommand.Execute(message);
    }
}
