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
        var button = sender as Button;
        var selected = button?.DataContext as Specialty;
        if (selected == null) return;
        App.DbContext.Specialties.Remove(selected);
        App.DbContext.SaveChanges();
        RefreshData();
    }
    private async void CreateNewSpecialty(object? sender, RoutedEventArgs e)
    {
        var window = new SpecialtyEditWindow(this);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
    private async void EditSpecialty(object? sender, TappedEventArgs e)
    {
        var window = new SpecialtyEditWindow(this, SpecialtyDataGrid.SelectedItem as Specialty);
        await window.ShowDialog<bool>(App.MainWindowLink);
    }
}