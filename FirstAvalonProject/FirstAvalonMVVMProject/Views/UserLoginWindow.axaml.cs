using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FirstAvalonMVVMProject.Data;
using FirstAvalonMVVMProject.Models;
using FirstAvalonMVVMProject.ViewModels;

namespace FirstAvalonMVVMProject.Views;

public partial class UserLoginWindow : Window
{
    public UserLoginWindow()
    {
        InitializeComponent();
        DataContext = new LoginWindowViewModel();
    }

    private async void Table_Double_Click(object? sender, TappedEventArgs e)
    {
        var selectedLogin = LoginDataGrid.SelectedItem as Login;

        if (selectedLogin == null) return;

        LoginVariableData.selectedLogin = selectedLogin;

        var createAndChangeLoginWindow = new CreateAndChangeLoginWindow(UserVariableData.selectedUserInMainWindow.IdUser);
        await createAndChangeLoginWindow.ShowDialog(this);
        
        var viewModel = DataContext as LoginWindowViewModel;
        viewModel.RefreshData();
    }

    private void Button_Delete_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selectedLogin = button?.DataContext as Login;
        // var selectedUser = MainDataGridUsers.SelectedItem as User;

        Console.WriteLine((selectedLogin == null) ? "User not found" : "User founded");

        if (selectedLogin == null) return;

        LoginVariableData.selectedLogin = selectedLogin;
        
        App.DbContext.Logins.Remove(selectedLogin);
        App.DbContext.SaveChanges();
        
        var viewModel = DataContext as LoginWindowViewModel;
        viewModel.RefreshData();
    }

    private async void Button_Create_OnClick(object? sender, RoutedEventArgs e)
    {
        LoginVariableData.selectedLogin = null;
        
        var createAndChangeLoginWindow = new CreateAndChangeLoginWindow(UserVariableData.selectedUserInMainWindow.IdUser);
        await createAndChangeLoginWindow.ShowDialog(this);
        
        var viewModel = DataContext as LoginWindowViewModel;
        viewModel.RefreshData();
    }
}