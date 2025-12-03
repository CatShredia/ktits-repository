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

public partial class DepartmentControl : UserControl
{
    private List<Department> _allDepartments = new();
    private string _selectedColumn = "All";

    public DepartmentControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "ID", "Cipher", "Name", "Faculty ID" };
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
        _allDepartments = App.DbContext.Departments.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allDepartments.Where(d =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;

            return _selectedColumn switch
            {
                "All" =>
                    d.Cipher?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    d.NameDepart?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    d.IdFaculty?.ToString().Contains(query) == true ||
                    d.IdDepart.ToString().Contains(query),
                "ID" => d.IdDepart.ToString().Contains(query),
                "Cipher" => d.Cipher?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Name" => d.NameDepart?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Faculty ID" => d.IdFaculty?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();

        DepartmentDataGrid.ItemsSource = filtered;
    }

    private void DeleteDepartment(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
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
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
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
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
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