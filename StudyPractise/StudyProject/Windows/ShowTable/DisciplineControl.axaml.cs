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

public partial class DisciplineControl : UserControl
{
    private List<Discipline> _allDisciplines = new();
    private string _selectedColumn = "All";

    public DisciplineControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "ID", "Code", "Hours", "Name", "Dept ID" };
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
        _allDisciplines = App.DbContext.Disciplines.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allDisciplines.Where(d =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return _selectedColumn switch
            {
                "All" =>
                    d.IdDiscipline.ToString().Contains(query) ||
                    d.Code?.ToString().Contains(query) == true ||
                    d.Hours?.ToString().Contains(query) == true ||
                    d.NameDisc?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    d.IdDepart?.ToString().Contains(query) == true,
                "ID" => d.IdDiscipline.ToString().Contains(query),
                "Code" => d.Code?.ToString().Contains(query) == true,
                "Hours" => d.Hours?.ToString().Contains(query) == true,
                "Name" => d.NameDisc?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Dept ID" => d.IdDepart?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();
        DisciplineDataGrid.ItemsSource = filtered;
    }

    private void DeleteDiscipline(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 4) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Discipline;
            if (selected == null) return;
            App.DbContext.Disciplines.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewDiscipline(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 4) && App.UserVariable != null)
        {
            var window = new DisciplineEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditDiscipline(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 4) && App.UserVariable != null)
        {
            var window = new DisciplineEditWindow(this, DisciplineDataGrid.SelectedItem as Discipline);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}