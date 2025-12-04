using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;

namespace StudyProject.Windows.ShowTable;
public partial class ExamControl : UserControl
{
    private List<Exam> _allExams = new();
    private string _selectedColumn = "All";

    public ExamControl()
    {
        InitializeComponent();
        SetupSearchColumns();
        RefreshData();
    }

    private void SetupSearchColumns()
    {
        var columns = new[] { "All", "Exam Date", "Discipline Code", "Student Reg", "Examiner Tab", "Classroom", "Grade" };
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
        _allExams = App.DbContext.Exams.ToList();
        ApplyFilter(SearchBox.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        => ApplyFilter(SearchBox.Text);

    private void ApplyFilter(string query)
    {
        var filtered = _allExams.Where(e =>
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return _selectedColumn switch
            {
                "All" =>
                    e.ExamDate?.ToString().Contains(query) == true ||
                    e.DisciplineCode?.ToString().Contains(query) == true ||
                    e.StudentReg?.ToString().Contains(query) == true ||
                    e.ExaminerTab?.ToString().Contains(query) == true ||
                    e.Classroom?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    e.Grade?.ToString().Contains(query) == true,
                "Exam Date" => e.ExamDate?.ToString().Contains(query) == true,
                "Discipline Code" => e.DisciplineCode?.ToString().Contains(query) == true,
                "Student Reg" => e.StudentReg?.ToString().Contains(query) == true,
                "Examiner Tab" => e.ExaminerTab?.ToString().Contains(query) == true,
                "Classroom" => e.Classroom?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                "Grade" => e.Grade?.ToString().Contains(query) == true,
                _ => false
            };
        }).ToList();
        ExamDataGrid.ItemsSource = filtered;
    }

    private void DeleteExam(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 3 || idRole == 4) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selectedExam = button?.DataContext as Exam;
            if (selectedExam == null) return;
            App.DbContext.Exams.Remove(selectedExam);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewExam(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 3 || idRole == 4) && App.UserVariable != null)
        {
            var window = new ExamEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditExam(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable?.authorizedLogin.IdUserNavigation.IdRole ?? 0;
        if ((idRole == 1 || idRole == 3 || idRole == 4) && App.UserVariable != null)
        {
            var window = new ExamEditWindow(this, ExamDataGrid.SelectedItem as Exam);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}