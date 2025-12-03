using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using StudyProject.Data;
using StudyProject.Parts;
using StudyProject.Variables;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Windows.EditWindows;

public partial class UserLoginAccountWindow : Window
{
    
    public Header _control { get; set; }
    
    public UserLoginAccountWindow(Header control)
    {
        InitializeComponent();
        
        _control = control;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(LoginBox.Text))
        {
            ErrorMessage.Text = "Login is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordBox.Text))
        {
            ErrorMessage.Text = "Password is required.";
            return;
        }

        var selectedLogin = App.DbContext.Logins.Include(login => login.IdUserNavigation)
            .FirstOrDefault(login => login.Login1 == LoginBox.Text);
        if (selectedLogin == null)
        {
            ErrorMessage.Text = "Username is not found.";
        }
        else if (selectedLogin.Password == PasswordBox.Text)
        {
            UserVariable.authorizedLogin = selectedLogin;
        }

        _control.RefreshDate();
        
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}