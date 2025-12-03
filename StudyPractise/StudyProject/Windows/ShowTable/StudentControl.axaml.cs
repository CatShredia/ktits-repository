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
    public StudentControl()
    {
        InitializeComponent();
        RefreshData();
    }

    public void RefreshData()
    {
        var students = App.DbContext.Students.ToList();
        StudentDataGrid.ItemsSource = students;
    }

    private void DeleteStudent(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
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
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
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
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
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