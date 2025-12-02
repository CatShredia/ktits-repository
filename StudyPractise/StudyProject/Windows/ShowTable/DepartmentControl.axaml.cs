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
        var button = sender as Button;
        var selected = button?.DataContext as Department;
        if (selected == null) return;
        App.DbContext.Departments.Remove(selected);
        App.DbContext.SaveChanges();
        RefreshData();
    }
    private async void CreateNewDepartment(object? sender, RoutedEventArgs e)
    {
        var window = new DepartmentEditWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
    private async void EditDepartment(object? sender, TappedEventArgs e)
    {
        var window = new DepartmentEditWindow(this, DepartmentDataGrid.SelectedItem as Department);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}