using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FirstAvalonMVVMProject.Data;
using FirstAvalonMVVMProject.Models;

namespace FirstAvalonMVVMProject.Views;

public partial class CreateAndChangeLoginWindow : Window
{
    
    private int? targetUserId;
    
    public CreateAndChangeLoginWindow(int ?targetUserId)
    {
        InitializeComponent();
        this.targetUserId = targetUserId;
        
        if (LoginVariableData.selectedLogin != null)
        {
            LoginTextBox.Text = LoginVariableData.selectedLogin.Login1;
            PasswordTextBox.Text = LoginVariableData.selectedLogin.Password;
        }
    }

    private void Submit_Button(object? sender, RoutedEventArgs e)
    {
        if(LoginVariableData.selectedLogin != null)
        {
            var idUser = LoginVariableData.selectedLogin.IdLogin;
            var selectedLogin = App.DbContext.Logins.FirstOrDefault(x => x.IdUser == idUser);

            if (selectedLogin == null) return;

            selectedLogin.Login1 = LoginTextBox.Text;
            selectedLogin.Password = PasswordTextBox.Text;
            selectedLogin.IdUser = idUser;
        }
        else
        {
            var newLogin = new Login()
            {
                Login1 = LoginTextBox.Text,
                Password = PasswordTextBox.Text,
                IdUser = targetUserId
            };
            App.DbContext.Logins.Add(newLogin);
        }
        
        
        App.DbContext.SaveChanges();
        this.Close();
    }
}