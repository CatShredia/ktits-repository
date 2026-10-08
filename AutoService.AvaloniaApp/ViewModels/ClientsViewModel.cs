using System.Collections.ObjectModel;
using AutoService.AvaloniaApp.Input;
using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

// Список и карточка клиентов.
// Создаётся в AppComposition и показывается в ClientsView.
public partial class ClientsViewModel : PagedViewModel
{
    private readonly AdminService _admin;

    public ClientsViewModel(AdminService admin) => _admin = admin;

    public ObservableCollection<ClientRow> Items { get; } = [];
    public string[] Fields { get; } = ["Все поля", "Имя", "Телефон", "Почта"];

    [ObservableProperty] private ClientRow? selected;
    [ObservableProperty] private string search = "";
    [ObservableProperty] private string field = "Все поля";
    [ObservableProperty] private int? editingId;
    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string phone = "";
    [ObservableProperty] private string email = "";

    partial void OnSelectedChanged(ClientRow? value)
    {
        if (value is null)
            return;
        EditingId = value.Id;
        FullName = value.FullName;
        Phone = value.Phone;
        Email = value.Email;
    }

    public override Task LoadAsync() => RunAsync(async () =>
    {
        var result = await _admin.GetClientsAsync(Search, FieldKey(Field), Page, PageSize);
        ApplyPage(result, items =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        });
        MarkReady();
    });

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private void NewClient()
    {
        Selected = null;
        EditingId = null;
        FullName = "";
        Phone = "";
        Email = "";
        Error = "";
    }

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        await _admin.SaveClientAsync(EditingId, FullName, Phone, Email);
        await LoadAsync();
    });

    [RelayCommand]
    private Task DeleteAsync() => RunAsync(async () =>
    {
        if (EditingId is not int id)
            throw new WorkshopException("Выберите клиента.");
        await _admin.DeleteClientAsync(id);
        NewClient();
        await LoadAsync();
    });

    private static string FieldKey(string title) => title switch
    {
        "Имя" => "name",
        "Телефон" => "phone",
        "Почта" => "email",
        _ => "all"
    };
}

// Список и карточка автомобилей.
// Создаётся в AppComposition и показывается в CarsView.
public partial class CarsViewModel : PagedViewModel
{
    private readonly AdminService _admin;

    public CarsViewModel(AdminService admin) => _admin = admin;

    public ObservableCollection<CarRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Clients { get; } = [];
    public ObservableCollection<LookupItem> Brands { get; } = [];
    public ObservableCollection<LookupItem> Categories { get; } = [];
    public string[] Fields { get; } = ["Все поля", "VIN", "Марка", "Модель", "Госномер"];

    [ObservableProperty] private string search = "";
    [ObservableProperty] private string field = "Все поля";
    [ObservableProperty] private CarRow? selected;
    [ObservableProperty] private int? editingId;
    [ObservableProperty] private LookupItem? client;
    [ObservableProperty] private LookupItem? brand;
    [ObservableProperty] private LookupItem? category;
    [ObservableProperty] private string model = "";
    [ObservableProperty] private string year = "";
    [ObservableProperty] private string vin = "";
    [ObservableProperty] private string plate = "";
    [ObservableProperty] private string mileage = "";
    [ObservableProperty] private string color = "";
    [ObservableProperty] private string notes = "";

    partial void OnSelectedChanged(CarRow? value)
    {
        if (value is null)
            return;
        EditingId = value.Id;
        Client = Clients.FirstOrDefault(x => x.Id == value.ClientId);
        Brand = Brands.FirstOrDefault(x => x.Id == value.BrandId);
        Category = Categories.FirstOrDefault(x => x.Id == value.CategoryId);
        Model = value.Model;
        Year = value.Year.ToString();
        Vin = value.Vin;
        Plate = value.LicensePlate;
        Mileage = value.Mileage.ToString();
        Color = value.Color;
        Notes = value.Notes;
    }

    public override Task LoadAsync() => RunAsync(async () =>
    {
        await Fill(Clients, () => _admin.GetClientsLookupAsync());
        await Fill(Brands, () => _admin.GetBrandsAsync());
        await Fill(Categories, () => _admin.GetCategoriesAsync());
        var result = await _admin.GetCarsAsync(Search, FieldKey(Field), Page, PageSize);
        ApplyPage(result, items =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        });
        MarkReady();
    });

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private void NewCar()
    {
        Selected = null;
        EditingId = null;
        Client = null;
        Brand = null;
        Category = null;
        Model = Year = Vin = Plate = Mileage = Color = Notes = "";
        Error = "";
    }

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        if (Client is null || Brand is null || Category is null)
            throw new WorkshopException("Выберите клиента, марку и категорию.");
        if (!InputValues.TryInt(Year, out var yearValue) || !InputValues.TryInt(Mileage, out var mileageValue))
            throw new WorkshopException("Год и пробег должны быть целыми числами.");
        await _admin.SaveCarAsync(EditingId, Client.Id, Brand.Id, Category.Id, Model, yearValue, Vin, Plate, mileageValue, Color, Notes);
        await LoadAsync();
    });

    [RelayCommand]
    private Task DeleteAsync() => RunAsync(async () =>
    {
        if (EditingId is not int id)
            throw new WorkshopException("Выберите автомобиль.");
        await _admin.DeleteCarAsync(id);
        NewCar();
        await LoadAsync();
    });

    private static async Task Fill(ObservableCollection<LookupItem> target, Func<Task<IReadOnlyList<LookupItem>>> load)
    {
        var selectedId = target.Count;
        _ = selectedId;
        var items = await load();
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }

    private static string FieldKey(string title) => title switch
    {
        "VIN" => "vin",
        "Марка" => "brand",
        "Модель" => "model",
        "Госномер" => "plate",
        _ => "all"
    };
}
