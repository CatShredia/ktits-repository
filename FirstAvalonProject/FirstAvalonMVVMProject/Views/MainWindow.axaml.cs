using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using FirstAvalonMVVMProject.Data;
using FirstAvalonMVVMProject.Models;
using FirstAvalonMVVMProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FirstAvalonMVVMProject.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private async void Table_Double_Tab(object? sender, TappedEventArgs e)
    {
        var selectedUser = MainDataGridUsers.SelectedItem as User;

        if (selectedUser == null) return;

        UserVariableData.selectedUserInMainWindow = selectedUser;

        var createAndChangeUserWindow = new CreateAndChangeUserWindow();
        await createAndChangeUserWindow.ShowDialog(this);
        
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.RefreshData();
        
        Console.WriteLine("Hi");
    }

    private async void Button_Create_OnClick(object? sender, RoutedEventArgs e)
    {
        UserVariableData.selectedUserInMainWindow = null;
        
        var createAndChangeUserWindow = new CreateAndChangeUserWindow();
        await createAndChangeUserWindow.ShowDialog(this);
        
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.RefreshData();
    }

    private void Button_Delete_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Deleting!");
        
        var button = sender as Button;
        var selectedUser = button?.DataContext as User;
        // var selectedUser = MainDataGridUsers.SelectedItem as User;

        Console.WriteLine((selectedUser == null) ? "User not found" : "User founded");

        if (selectedUser == null) return;

        UserVariableData.selectedUserInMainWindow = selectedUser;
        
        App.DbContext.Users.Remove(selectedUser);
        App.DbContext.SaveChanges();
        
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.RefreshData();
    }
}