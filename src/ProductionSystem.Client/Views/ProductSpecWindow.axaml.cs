using Avalonia.Controls;
using Avalonia.Interactivity;
using ProductionSystem.Client.Services;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class ProductSpecWindow : Window
{
    private readonly ProductSpecViewModel _vm;

    public ProductSpecWindow(BackendApi api, string productName, bool canEdit)
    {
        InitializeComponent();
        _vm = new ProductSpecViewModel(api, productName, canEdit);
        DataContext = _vm;
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (await _vm.SaveAsync())
            Close();
    }

    private void Close_Click(object? sender, RoutedEventArgs e) => Close();
}
