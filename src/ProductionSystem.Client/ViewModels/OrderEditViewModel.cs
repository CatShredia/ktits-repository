using System.Collections.ObjectModel;
using ProductionSystem.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class OrderEditViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly string? _orderNumber;

    public ObservableCollection<CustomerUserDto> Customers { get; } = new();
    public ObservableCollection<OrderDimensionDto> Dimensions { get; } = new();

    [ObservableProperty] private string _orderName = "";
    [ObservableProperty] private string _productDescription = "";
    [ObservableProperty] private CustomerUserDto? _selectedCustomer;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _dimDescription = "";
    [ObservableProperty] private string _dimUnit = "мм";
    [ObservableProperty] private string _dimValueText = "";

    public bool IsManager => _api.Role == UserRoles.Manager;
    public bool IsEdit => _orderNumber is not null;

    public OrderEditViewModel(BackendApi api, string? orderNumber)
    {
        _api = api;
        _orderNumber = orderNumber;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        if (IsManager)
        {
            var customers = await _api.GetCustomersAsync();
            Customers.Clear();
            if (customers != null)
            {
                foreach (var c in customers)
                    Customers.Add(c);
            }
        }

        if (_orderNumber is null)
            return;

        var detail = await _api.GetOrderAsync(_orderNumber);
        if (detail is null)
            return;

        OrderName = detail.OrderName;
        ProductDescription = detail.ProductDescription;
        SelectedCustomer = Customers.FirstOrDefault(c => c.Login == detail.CustomerLogin);
        Dimensions.Clear();
        foreach (var d in detail.Dimensions)
            Dimensions.Add(d);
    }

    [RelayCommand]
    private void AddDimension()
    {
        if (!decimal.TryParse(DimValueText.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v))
        {
            ErrorMessage = "Некорректное значение замера.";
            return;
        }

        Dimensions.Add(new OrderDimensionDto
        {
            Description = DimDescription.Trim(),
            Unit = DimUnit.Trim(),
            Value = v,
        });
        DimDescription = "";
        DimValueText = "";
        ErrorMessage = null;
    }

    [RelayCommand]
    public async Task<bool> SaveAsync()
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(OrderName))
        {
            ErrorMessage = "Укажите наименование заказа.";
            return false;
        }

        var body = new OrderCreateRequest
        {
            OrderName = OrderName.Trim(),
            ProductDescription = ProductDescription.Trim(),
            CustomerLogin = IsManager ? SelectedCustomer?.Login : null,
            Dimensions = Dimensions.ToList(),
        };

        if (IsManager && string.IsNullOrWhiteSpace(body.CustomerLogin))
        {
            ErrorMessage = "Выберите заказчика.";
            return false;
        }

        if (_orderNumber is null)
        {
            var (ok, err, _) = await _api.CreateOrderAsync(body);
            if (!ok)
            {
                ErrorMessage = err;
                return false;
            }
        }
        else
        {
            var (ok, err) = await _api.UpdateOrderAsync(_orderNumber, body);
            if (!ok)
            {
                ErrorMessage = err;
                return false;
            }
        }

        return true;
    }
}
