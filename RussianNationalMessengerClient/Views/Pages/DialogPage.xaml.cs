using RussianNationalMessengerClient.Dtos;
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
}
