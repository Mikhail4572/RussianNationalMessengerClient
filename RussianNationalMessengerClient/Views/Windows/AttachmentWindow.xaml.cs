using RussianNationalMessengerClient.ViewModels;
using System.Windows;

namespace RussianNationalMessengerClient.Views.Windows;

public partial class AttachmentWindow : Window
{
    public AttachmentWindow(AttachmentWindowViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm;
    }
}
