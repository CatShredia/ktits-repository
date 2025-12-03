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

        // right part
        LoginButton.IsVisible = !isAuth;
        // RegisterButton.IsVisible = !isAuth;
        ProfileButton.IsVisible = isAuth;
        OutButton.IsVisible = isAuth;

        ProfileButton.Content = isAuth ? App.UserVariable.authorizedLogin.Login1 : "";

        // left part
        ExamButton.IsVisible = false;
        SpecialtyButton.IsVisible = false;
        ExamButton.IsVisible = false;
        StudentButton.IsVisible = false;
        if (isAuth)
        {
            // Exam all can see
            ExamButton.IsVisible =
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 1 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 2 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 3 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 4
                    ? true
                    : false;
            
            // Specialty all can see
            SpecialtyButton.IsVisible =
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 1 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 2 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 3 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 4
                    ? true
                    : false;
            
            // Employee
            EmployeeButton.IsVisible =
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 3 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 4
                    ? true
                    : false;
            
            // Students
            StudentButton.IsVisible =
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 2 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 4
                    ? true
                    : false;
            
            DepartmentButton.IsVisible =
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 1 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 2 ||
                App.UserVariable.authorizedLogin.IdUserNavigation.IdRole == 4
                    ? true
                    : false;
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
}