using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Windows.EditWindows;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Parts;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    private void ToExamTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new ExamControl());
    }

    private void ToSpecialtyTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new SpecialtyControl());
    }

    private void ToDepartmentTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new DepartmentControl());
    }

    private void ToEmployeeTable(object? sender, RoutedEventArgs e)
    {
        App.MainWindowLink.UpdateContent(new EmployeeControl());
    }

    private void ToLogin(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
    
    private async void ToRegister(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow();
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}