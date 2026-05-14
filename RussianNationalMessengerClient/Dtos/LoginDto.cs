using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RussianNationalMessengerClient.Dtos;

public class LoginDto : INotifyPropertyChanged
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "dima";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "2222";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
