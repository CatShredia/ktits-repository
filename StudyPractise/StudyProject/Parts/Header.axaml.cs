using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Variables;
using StudyProject.Windows.EditWindows;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Parts;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    
        ProfileName.Text = "";
        RefreshDate();
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

    private async void ToLogin(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginAccountWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }

    private async void ToRegister(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow();
        await window.ShowDialog<bool>(App.MainWindowLink);
    }

    private void ToOut(object? sender, RoutedEventArgs e)
    {
        UserVariable.authorizedLogin = null;
        RefreshDate();
    }

    public void RefreshDate()
    {
        bool isAuth = UserVariable.authorizedLogin != null;
        LoginButton.Opacity = isAuth ? 0 : 1;
        RegisterButton.Opacity = isAuth ? 0 : 1;
        OutButton.Opacity = isAuth ? 1 : 0;

        ProfileName.Text = isAuth ? UserVariable.authorizedLogin.Login1 : "";
    }
}