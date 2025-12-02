using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using Application = Avalonia.Application;

namespace StudyProject;

public partial class App : Application
{
    public static AppDbContext DbContext { get; private set; } = new AppDbContext();
    
    public static MainWindow? MainWindowLink { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindowLink = new MainWindow();
            desktop.MainWindow = MainWindowLink;
        }

        base.OnFrameworkInitializationCompleted();
    }
}