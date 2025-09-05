using Avalonia.Controls;
using FirstAvalonMVVMProject.ViewModels;

namespace FirstAvalonMVVMProject.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}