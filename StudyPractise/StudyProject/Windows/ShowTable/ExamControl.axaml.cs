using System;
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
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selectedExam = button?.DataContext as Exam;

            Console.WriteLine((selectedExam == null) ? "Exam not found" : "Exam founded");

            if (selectedExam == null) return;

            App.DbContext.Exams.Remove(selectedExam);
            App.DbContext.SaveChanges();

            RefreshDate();
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
            var result = await newEditWindow.ShowDialog<bool>(App.MainWindowLink);
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
            var result = await newEditWindow.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}