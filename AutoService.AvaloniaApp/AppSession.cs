using AutoService.Data.Workshop;

namespace AutoService.AvaloniaApp;

// Текущий вошедший пользователь.
// Записывается в ShellViewModel, читается экранами ролей.
public sealed class AppSession
{
    public CurrentUser? User { get; set; }
}
