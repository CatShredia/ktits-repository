using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Variables;
using StudyProject.Windows.EditWindows;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Parts;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    
        ProfileButton.Content = "";
        RefreshDate();
    }

    private void ToExamTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new ExamControl());
    }

    private void ToSpecialtyTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new SpecialtyControl());
    }

    private void ToDepartmentTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new DepartmentControl());
    }

    private void ToEmployeeTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new EmployeeControl());
    }

    private async void ToLogin(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginAccountWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }

    private async void ToRegister(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow();
        await window.ShowDialog<bool>(App.MainWindowLink);
    }

    private void ToOut(object? sender, RoutedEventArgs e)
    {
        UserVariable.authorizedLogin = null;
        RefreshDate();
    }

    public void RefreshDate()
    {
        bool isAuth = UserVariable.authorizedLogin != null;
        LoginButton.IsVisible = !isAuth;
        RegisterButton.IsVisible = !isAuth;
        ProfileButton.IsVisible = isAuth;
        OutButton.IsVisible = isAuth;

        ProfileButton.Content = isAuth ? UserVariable.authorizedLogin.Login1 : "";
        
        Console.WriteLine(UserVariable.authorizedLogin);
    }

    private async void ToProfile(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow(UserVariable.authorizedLogin.IdUserNavigation, null);
        await window.ShowDialog<bool>(App.MainWindowLink);
        
        RefreshDate();
        
    }
}