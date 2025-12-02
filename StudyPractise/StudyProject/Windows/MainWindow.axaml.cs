using Avalonia.Controls;

namespace StudyProject;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public void UpdateContent(UserControl control)
    {
        ContentGrid.Children.Clear();
        ContentGrid.Children.Add(control);
    }
}