using System;
using Avalonia.Controls;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        Opened += LoginWindow_Opened;
    }

    private async void LoginWindow_Opened(object? sender, EventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            await vm.TryAutoLoginAsync();
    }
}
