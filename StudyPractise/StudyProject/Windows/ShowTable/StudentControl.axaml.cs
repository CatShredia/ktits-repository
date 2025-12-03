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
        var button = sender as Button;
        var selected = button?.DataContext as Student;
        if (selected == null) return;
        App.DbContext.Students.Remove(selected);
        App.DbContext.SaveChanges();
        RefreshData();
    }
    private async void CreateNewStudent(object? sender, RoutedEventArgs e)
    {
        var window = new StudentEditWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
    private async void EditStudent(object? sender, TappedEventArgs e)
    {
        var window = new StudentEditWindow(this, StudentDataGrid.SelectedItem as Student);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}