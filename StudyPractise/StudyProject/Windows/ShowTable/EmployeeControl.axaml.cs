using System;
using System.Collections.Generic;
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
    private List<Employee> _allEmployees = new();
    private string _selectedColumn = "All";

    public EmployeeControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "Tab Num", "Fullname", "Position", "Salary", "Dept ID", "Chief Tab" };
        SearchColumnBox.Items.Clear();
        foreach (var col in columns)
            SearchColumnBox.Items.Add(col);
        SearchColumnBox.SelectedIndex = 0;
        SearchColumnBox.SelectionChanged += (s, e) =>
        {
            _selectedColumn = SearchColumnBox.SelectedItem?.ToString() ?? "All";
            ApplyFilter(SearchBox.Text);
        };
    }

    public void RefreshData()
    {
        _allEmployees = App.DbContext.Employees.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allEmployees.Where(e =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return _selectedColumn switch
            {
                "All" =>
                    e.TabNumEmployee.ToString().Contains(query) ||
                    e.Fullname?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    e.PositionEmp?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    e.Salary?.ToString().Contains(query) == true ||
                    e.IdDepart?.ToString().Contains(query) == true ||
                    e.Chief?.ToString().Contains(query) == true,
                "Tab Num" => e.TabNumEmployee.ToString().Contains(query),
                "Fullname" => e.Fullname?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Position" => e.PositionEmp?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Salary" => e.Salary?.ToString().Contains(query) == true,
                "Dept ID" => e.IdDepart?.ToString().Contains(query) == true,
                "Chief Tab" => e.Chief?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();
        EmployeeDataGrid.ItemsSource = filtered;
    }

    private void DeleteEmployee(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Employee;
            if (selected == null) return;
            App.DbContext.Employees.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewEmployee(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new EmployeeEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditEmployee(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new EmployeeEditWindow(this, EmployeeDataGrid.SelectedItem as Employee);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}