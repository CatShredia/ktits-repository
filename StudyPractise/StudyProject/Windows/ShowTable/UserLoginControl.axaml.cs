using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;
namespace StudyProject.Windows.ShowTable;
public partial class UserLoginControl : UserControl
{
    public UserLoginControl()
    {
        InitializeComponent();
        RefreshData();
    }
    public void RefreshData()
    {
        var users = App.DbContext.Users
            .Include(u => u.Logins)
            .ToList();
        UserDataGrid.ItemsSource = users;
    }
    private void DeleteUserLogin(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selected = button?.DataContext as User;
        if (selected == null) return;
        App.DbContext.Users.Remove(selected);
        App.DbContext.SaveChanges();
        RefreshData();
    }
    private async void CreateNewUserLogin(object? sender, RoutedEventArgs e)
    {
        var window = new UserLoginEditWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
    private async void EditUserLogin(object? sender, TappedEventArgs e)
    {
        var window = new UserLoginEditWindow(this, UserDataGrid.SelectedItem as User);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}