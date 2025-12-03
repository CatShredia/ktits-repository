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
public partial class SpecialtyControl : UserControl
{
    private List<Specialty> _allSpecialties = new();
    private string _selectedColumn = "All";

    public SpecialtyControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "ID", "Code", "Direction", "Dept ID" };
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
        _allSpecialties = App.DbContext.Specialties.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allSpecialties.Where(s =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return _selectedColumn switch
            {
                "All" =>
                    s.IdSpecialty.ToString().Contains(query) ||
                    s.Code?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    s.Direction?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    s.IdDepart?.ToString().Contains(query) == true,
                "ID" => s.IdSpecialty.ToString().Contains(query),
                "Code" => s.Code?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Direction" => s.Direction?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Dept ID" => s.IdDepart?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();
        SpecialtyDataGrid.ItemsSource = filtered;
    }

    private void DeleteSpecialty(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Specialty;
            if (selected == null) return;
            App.DbContext.Specialties.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewSpecialty(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new SpecialtyEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditSpecialty(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new SpecialtyEditWindow(this, SpecialtyDataGrid.SelectedItem as Specialty);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}