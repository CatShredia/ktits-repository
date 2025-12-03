using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;

namespace StudyProject.Windows.ShowTable;

public partial class DepartmentControl : UserControl
{
    public DepartmentControl()
    {
        InitializeComponent();
        RefreshData();
    }

    public void RefreshData()
    {
        var departments = App.DbContext.Departments.ToList();
        DepartmentDataGrid.ItemsSource = departments;
    }

    private void DeleteDepartment(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Department;
            if (selected == null) return;
            App.DbContext.Departments.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewDepartment(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new DepartmentEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditDepartment(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new DepartmentEditWindow(this, DepartmentDataGrid.SelectedItem as Department);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}