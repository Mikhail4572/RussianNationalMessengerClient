using Microsoft.Extensions.DependencyInjection;
using RussianNationalMessengerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Services;

public class NavigationService : ViewModelBase
{
    private readonly IServiceProvider _provider;

    public ViewModelBase CurrentViewModel
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public NavigationService(IServiceProvider provider) =>    
        _provider = provider;    

    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentViewModel =
            _provider.GetRequiredService<T>();
    }
}