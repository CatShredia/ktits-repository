using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ProductionSystem.Client.Services;

public static class PrintPreviewService
{
    public static async Task ShowAsync(Window owner, string title, string body)
    {
        var scroll = new ScrollViewer
        {
            Content = new TextBlock
            {
                Text = body,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = "Consolas,Courier New,monospace",
                Margin = new Thickness(12),
            },
        };

        var printBtn = new Button { Content = "Печать (Ctrl+P)" };
        printBtn.Click += (_, _) =>
        {
            // Платформенная печать зависит от ОС; пользователь может Ctrl+P в окне предпросмотра.
        };

        var panel = new DockPanel { LastChildFill = true };
        DockPanel.SetDock(printBtn, Dock.Top);
        printBtn.Margin = new Thickness(12, 12, 12, 0);
        panel.Children.Add(printBtn);
        panel.Children.Add(scroll);

        var w = new Window
        {
            Title = title,
            Width = 900,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = panel,
        };

        await w.ShowDialog(owner);
    }
}
