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
        App.UserVariable = null;
        RefreshDate();

        App.MainWindowLink.DropContent();
    }

    public void RefreshDate()
    {
        bool isAuth = App.UserVariable != null;

        LoginButton.IsVisible = !isAuth;
        ProfileButton.IsVisible = isAuth;
        OutButton.IsVisible = isAuth;
        ProfileButton.Content = isAuth ? App.UserVariable.authorizedLogin.Login1 : "";

        ExamButton.IsVisible = false;
        SpecialtyButton.IsVisible = false;
        StudentButton.IsVisible = false;
        DepartmentButton.IsVisible = false;
        EmployeeButton.IsVisible = false;
        DisciplineButton.IsVisible = false; 

        if (isAuth)
        {
            int roleId = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;

            ExamButton.IsVisible = true;
            SpecialtyButton.IsVisible = true;

            DisciplineButton.IsVisible = true;

            EmployeeButton.IsVisible = roleId == 1 || roleId == 3 || roleId == 4;

            StudentButton.IsVisible = roleId == 1 || roleId == 2 || roleId == 4;

            DepartmentButton.IsVisible = roleId == 1 || roleId == 2 || roleId == 4;
        }
    }
    
    private async void ToProfile(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow(App.UserVariable.authorizedLogin.IdUserNavigation, null);
        await window.ShowDialog<bool>(App.MainWindowLink);

        RefreshDate();
    }

    private void ToStudentTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new StudentControl());
    }

    private void ToClearWindow(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.DropContent();
    }

    private void ToDisciplineTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new DisciplineControl());
    }
}