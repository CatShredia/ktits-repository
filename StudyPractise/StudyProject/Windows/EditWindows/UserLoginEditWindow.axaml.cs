using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Windows.EditWindows;

public partial class UserLoginEditWindow : Window
{
    private readonly User _user;
    private readonly Login? _login;
    private readonly bool _isNew;
    private readonly UserLoginControl _control;

    public UserLoginEditWindow()
    {
        _user = new User();
        _login = new Login();
        _isNew = true;
        InitializeComponent();
    }

    public UserLoginEditWindow(UserLoginControl control)
    {
        _user = new User();
        _login = new Login();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }

    public UserLoginEditWindow(UserLoginControl control, User user)
    {
        _user = user ?? throw new System.ArgumentNullException(nameof(user));
        _login = _user.Logins.FirstOrDefault();
        _isNew = false;
        _control = control;
        InitializeComponent();
        NameBox.Text = _user.Name;
        EmailBox.Text = _user.Email;
        RoleIdBox.Text = _user.IdRole.ToString();
        if (_login != null)
        {
            LoginBox.Text = _login.Login1;
            PasswordBox.Text = _login.Password;
        }
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            ErrorMessage.Text = "Name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailBox.Text))
        {
            ErrorMessage.Text = "Email is required.";
            return;
        }

        if (!int.TryParse(RoleIdBox.Text, out var roleId))
        {
            ErrorMessage.Text = "Role ID must be a valid integer.";
            return;
        }

        if (App.DbContext.Roles.Find(roleId) == null)
        {
            ErrorMessage.Text = $"Role with ID {roleId} does not exist.";
            return;
        }

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

        _user.Name = NameBox.Text.Trim();
        _user.Email = EmailBox.Text.Trim();
        _user.IdRole = roleId;
        if (_isNew)
        {
            App.DbContext.Users.Add(_user);
            App.DbContext.SaveChanges(); // получаем Id
        }

        if (_login == null)
        {
            var newLogin = new Login
            {
                Login1 = LoginBox.Text.Trim(),
                Password = PasswordBox.Text.Trim(),
                IdUser = _user.Id
            };
            App.DbContext.Logins.Add(newLogin);
        }
        else
        {
            _login.Login1 = LoginBox.Text.Trim();
            _login.Password = PasswordBox.Text.Trim();
        }

        App.DbContext.SaveChanges();
        if (_control != null)
        {
            _control.RefreshData();
        }

        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}