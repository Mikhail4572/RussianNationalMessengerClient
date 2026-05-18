using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.ViewModels;

public class ChatsViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ChatsViewModel(MainViewModel main)
    {
        _main = main;
    }
}
