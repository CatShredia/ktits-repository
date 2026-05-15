using System.Collections.ObjectModel;
using ProductionSystem.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;
using ProductionSystem.Client.Views;

namespace ProductionSystem.Client.ViewModels;

public partial class ProductsViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<ProductListItemDto> Items { get; } = new();

    [ObservableProperty] private ProductListItemDto? _selectedProduct;
    [ObservableProperty] private string? _statusMessage;

    public bool CanEdit => _api.Role == UserRoles.Foreman;

    public ProductsViewModel(BackendApi api)
    {
        _api = api;
        _ = RefreshAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var list = await _api.GetProductsAsync();
        Items.Clear();
        if (list != null)
        {
            foreach (var p in list)
                Items.Add(p);
        }
    }

    [RelayCommand]
    private async Task OpenSpecificationAsync()
    {
        if (SelectedProduct is null)
            return;

        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        var dlg = new ProductSpecWindow(_api, SelectedProduct.Name, CanEdit);
        await dlg.ShowDialog(owner);
        await RefreshAsync();
    }
}
