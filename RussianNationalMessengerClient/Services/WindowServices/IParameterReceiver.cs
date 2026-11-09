using System;
using System.Collections.Generic;
using System.Text;

namespace RussianNationalMessengerClient.Services.WindowServices;
public interface IParameterReceiver
{
    void Initialize(object parameter);
}
