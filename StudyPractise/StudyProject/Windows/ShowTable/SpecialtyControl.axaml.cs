using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using StudyProject.Data;
using StudyProject.Windows.EditWindows;

namespace StudyProject.Windows.ShowTable;

public partial class SpecialtyControl : UserControl
{
    public SpecialtyControl()
    {
        InitializeComponent();
        RefreshData();
    }

    public void RefreshData()
    {
        var specialties = App.DbContext.Specialties.ToList();
        SpecialtyDataGrid.ItemsSource = specialties;
    }

    private void DeleteSpecialty(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var button = sender as Button;
            var selected = button?.DataContext as Specialty;
            if (selected == null) return;
            App.DbContext.Specialties.Remove(selected);
            App.DbContext.SaveChanges();
            RefreshData();
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void CreateNewSpecialty(object? sender, RoutedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new SpecialtyEditWindow(this);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }

    private async void EditSpecialty(object? sender, TappedEventArgs e)
    {
        int idRole = App.UserVariable.authorizedLogin.IdUserNavigation.IdRole;
        if ((idRole == 2 || idRole == 4 || idRole == 3) && App.UserVariable != null)
        {
            var window = new SpecialtyEditWindow(this, SpecialtyDataGrid.SelectedItem as Specialty);
            await window.ShowDialog<bool>(App.MainWindowLink);
        }
        else
        {
            MessageBox.Text = "You haven't current permissions";
        }
    }
}