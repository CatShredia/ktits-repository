using Avalonia.Controls;
using Avalonia.Interactivity;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class OrderEditWindow : Window
{
    private readonly OrderEditViewModel _vm;

    public OrderEditWindow(BackendApi api, string? orderNumber)
    {
        InitializeComponent();
        _vm = new OrderEditViewModel(api, orderNumber);
        DataContext = _vm;
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (await _vm.SaveAsync())
            Close();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close();
}
