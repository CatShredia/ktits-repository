using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FirstAvalonMVVMProject.Data;
using FirstAvalonMVVMProject.Models;

namespace FirstAvalonMVVMProject.Views;

public partial class CreateAndChangeUser : Window
{
    public CreateAndChangeUser()
    {
        InitializeComponent();

        if (UserVariableData.selectedUserInMainWindow != null)
        {
            FistNameTextBox.Text = UserVariableData.selectedUserInMainWindow.FirstName;
            SecondNameTextBox.Text = UserVariableData.selectedUserInMainWindow.SecondName;
            DescriptionTextBox.Text = UserVariableData.selectedUserInMainWindow.Description;
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if(UserVariableData.selectedUserInMainWindow != null)
        {
            var idUser = UserVariableData.selectedUserInMainWindow.IdUser;
            var selectedUser = App.DbContext.Users.FirstOrDefault(x => x.IdUser == idUser);

            if (selectedUser == null) return;

            selectedUser.FirstName = FistNameTextBox.Text;
            selectedUser.SecondName = SecondNameTextBox.Text;
            selectedUser.Description = DescriptionTextBox.Text;
        }
        else
        {
            var newUser = new User()
            {
                FirstName = FistNameTextBox.Text,
                SecondName = SecondNameTextBox.Text,
                Description = DescriptionTextBox.Text,
            };
            App.DbContext.Users.Add(newUser);
        }
        
        
        App.DbContext.SaveChanges();
        this.Close();
    }
}