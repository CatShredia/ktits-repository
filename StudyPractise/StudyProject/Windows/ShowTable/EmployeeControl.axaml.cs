using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;
namespace StudyProject.Windows.ShowTable;
public partial class EmployeeControl : UserControl
{
    public EmployeeControl()
    {
        InitializeComponent();
        RefreshData();
    }
    public void RefreshData()
    {
        var employees = App.DbContext.Employees.ToList();
        EmployeeDataGrid.ItemsSource = employees;
    }
    private void DeleteEmployee(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selected = button?.DataContext as Employee;
        if (selected == null) return;
        App.DbContext.Employees.Remove(selected);
        App.DbContext.SaveChanges();
        RefreshData();
    }
    private async void CreateNewEmployee(object? sender, RoutedEventArgs e)
    {
        var window = new EmployeeEditWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
    private async void EditEmployee(object? sender, TappedEventArgs e)
    {
        var window = new EmployeeEditWindow(this, EmployeeDataGrid.SelectedItem as Employee);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}