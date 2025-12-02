using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;

namespace StudyProject.Windows.ShowTable;

public partial class ExamControl : UserControl
{
    public ExamControl()
    {
        InitializeComponent();

        RefreshDate();
    }

    public void RefreshDate()
    {
        var examLists = App.DbContext.Exams
            .ToList();

        ExamDataGrid.ItemsSource = examLists;
    }

    private void DeleteExam(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selectedExam = button?.DataContext as Exam;

        Console.WriteLine((selectedExam == null) ? "Exam not found" : "Exam founded");

        if (selectedExam == null) return;

        App.DbContext.Exams.Remove(selectedExam);
        App.DbContext.SaveChanges();

        RefreshDate();
    }

    private async void CreateNewExam(object? sender, RoutedEventArgs e)
    {
        var newEditWindow = new ExamEditWindow();
        var result = await newEditWindow.ShowDialog<bool>(App.MainWindowLink);
    }
}