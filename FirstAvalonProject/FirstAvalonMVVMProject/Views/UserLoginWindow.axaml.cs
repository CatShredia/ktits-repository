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
        User selectedUser = UserVariableData.selectedUserInMainWindow;

        if (selectedUser == null) return;

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
}