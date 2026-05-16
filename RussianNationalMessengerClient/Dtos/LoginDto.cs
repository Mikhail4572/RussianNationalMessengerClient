using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class LoginDto : INotifyPropertyChanged
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "mikhail";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "1111";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
