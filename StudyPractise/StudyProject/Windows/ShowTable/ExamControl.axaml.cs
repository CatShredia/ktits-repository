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

    public ExamControl()
    {
        InitializeComponent();
        RefreshData();
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
        var filtered = _allExams
            .Where(e => string.IsNullOrWhiteSpace(query) ||
                        e.Classroom?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                        e.DisciplineCode?.ToString().Contains(query) == true ||
                        e.StudentReg?.ToString().Contains(query) == true ||
                        e.ExaminerTab?.ToString().Contains(query) == true ||
                        e.Grade?.ToString().Contains(query) == true ||
                        e.ExamDate?.ToString().Contains(query) == true)
            .ToList();
        ExamDataGrid.ItemsSource = filtered;
    }

    private void DeleteExam(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
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
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var newEditWindow = new ExamEditWindow(this);
            await newEditWindow.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditExam(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var newEditWindow = new ExamEditWindow(this, ExamDataGrid.SelectedItem as Exam);
            await newEditWindow.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}