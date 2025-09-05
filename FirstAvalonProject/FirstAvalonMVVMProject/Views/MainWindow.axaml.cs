using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using FirstAvalonMVVMProject.Data;
using FirstAvalonMVVMProject.Models;
using FirstAvalonMVVMProject.ViewModels;

namespace FirstAvalonMVVMProject.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private async void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var selectedUser = MainDataGridUsers.SelectedItem as User;

        if (selectedUser == null) return;

        UserVariableData.selectedUserInMainWindow = selectedUser;

        var createAndChangeUserWindow = new CreateAndChangeUser();
        await createAndChangeUserWindow.ShowDialog(this);
        
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.RefreshData();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        UserVariableData.selectedUserInMainWindow = null;
        
        var createAndChangeUserWindow = new CreateAndChangeUser();
        await createAndChangeUserWindow.ShowDialog(this);
        
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.RefreshData();
    }
}