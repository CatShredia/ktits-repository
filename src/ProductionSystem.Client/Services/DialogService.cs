using Avalonia.Controls;
using Avalonia.Layout;

namespace ProductionSystem.Client.Services;

public static class DialogService
{
    public static async Task<bool> ConfirmAsync(Window owner, string message)
    {
        var ok = false;
        var w = new Window
        {
            Title = "Подтверждение",
            Width = 440,
            Height = 160,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var panel = new StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
        };
        var yes = new Button { Content = "Да" };
        var no = new Button { Content = "Нет" };
        yes.Click += (_, _) => { ok = true; w.Close(); };
        no.Click += (_, _) => { ok = false; w.Close(); };
        row.Children.Add(yes);
        row.Children.Add(no);
        panel.Children.Add(row);
        w.Content = panel;

        await w.ShowDialog(owner);
        return ok;
    }

    public static async Task ShowInfoAsync(Window owner, string message)
    {
        var w = new Window
        {
            Title = "Сообщение",
            Width = 440,
            Height = 160,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        var panel = new StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
        var btn = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Right };
        btn.Click += (_, _) => w.Close();
        panel.Children.Add(btn);
        w.Content = panel;
        await w.ShowDialog(owner);
    }

    public static Window? TryGetMainWindow() =>
        Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime d
            ? d.MainWindow as Window
            : null;
}
