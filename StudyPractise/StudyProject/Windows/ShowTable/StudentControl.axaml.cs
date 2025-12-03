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
public partial class StudentControl : UserControl
{
    private List<Student> _allStudents = new();
    private string _selectedColumn = "All";

    public StudentControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "Reg Number", "Fullname", "Specialty ID" };
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
        _allStudents = App.DbContext.Students.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allStudents.Where(s =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return _selectedColumn switch
            {
                "All" =>
                    s.RegNumber.ToString().Contains(query) ||
                    s.Fullname?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    s.IdSpeciality?.ToString().Contains(query) == true,
                "Reg Number" => s.RegNumber.ToString().Contains(query),
                "Fullname" => s.Fullname?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Specialty ID" => s.IdSpeciality?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();
        StudentDataGrid.ItemsSource = filtered;
    }

    private void DeleteStudent(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Student;
            if (selected == null) return;
            App.DbContext.Students.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewStudent(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new StudentEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditStudent(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new StudentEditWindow(this, StudentDataGrid.SelectedItem as Student);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}