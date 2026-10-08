using System.Collections.ObjectModel;
using AutoService.AvaloniaApp.Input;
using AutoService.Core.Enums;
using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

// Услуга, отмеченная у механика в справочнике.
// Используется в CatalogViewModel.
public partial class ServiceChoice : ObservableObject
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    [ObservableProperty] private bool isSelected;
}

// Вид справочника в переключателе экрана.
// Используется в CatalogViewModel.
public sealed class CatalogOption
{
    public CatalogKind Kind { get; init; }
    public string Title { get; init; } = "";
    public override string ToString() => Title;
}

// Просмотр и правка справочников.
// Создаётся в AppComposition и показывается в CatalogView.
public partial class CatalogViewModel : AppPageViewModel
{
    private readonly AdminService _admin;
    private bool _listsReady;

    public CatalogViewModel(AdminService admin)
    {
        _admin = admin;
        kind = Kinds[0];
    }

    public CatalogOption[] Kinds { get; } =
    [
        new() { Kind = CatalogKind.Brands, Title = "Марки" },
        new() { Kind = CatalogKind.Categories, Title = "Категории" },
        new() { Kind = CatalogKind.Departments, Title = "Отделы" },
        new() { Kind = CatalogKind.Specializations, Title = "Специализации" },
        new() { Kind = CatalogKind.Services, Title = "Услуги" },
        new() { Kind = CatalogKind.Suppliers, Title = "Поставщики" },
        new() { Kind = CatalogKind.Parts, Title = "Запчасти" },
        new() { Kind = CatalogKind.Bays, Title = "Ремонтные места" },
        new() { Kind = CatalogKind.Mechanics, Title = "Механики" }
    ];

    public ObservableCollection<CatalogRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Suppliers { get; } = [];
    public ObservableCollection<LookupItem> Departments { get; } = [];
    public ObservableCollection<LookupItem> Specializations { get; } = [];
    public ObservableCollection<ServiceChoice> ServiceChoices { get; } = [];
    public RepairBayKind[] BayKinds { get; } = Enum.GetValues<RepairBayKind>();

    [ObservableProperty] private CatalogOption? kind;
    [ObservableProperty] private CatalogRow? selected;
    [ObservableProperty] private int? editingId;
    [ObservableProperty] private string name = "";
    [ObservableProperty] private string description = "";
    [ObservableProperty] private string phone = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string sku = "";
    [ObservableProperty] private string login = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string price = "";
    [ObservableProperty] private string purchasePrice = "";
    [ObservableProperty] private string duration = "";
    [ObservableProperty] private string quantity = "";
    [ObservableProperty] private string minQuantity = "";
    [ObservableProperty] private string search = "";
    [ObservableProperty] private string maxPrice = "";
    [ObservableProperty] private string maxDuration = "";
    [ObservableProperty] private bool belowMinimum;
    [ObservableProperty] private LookupItem? supplier;
    [ObservableProperty] private LookupItem? department;
    [ObservableProperty] private LookupItem? specialization;
    [ObservableProperty] private RepairBayKind bayKind = RepairBayKind.Подъёмник;
    [ObservableProperty] private int page = 1;
    [ObservableProperty] private int pageSize = 10;
    [ObservableProperty] private int totalCount;

    public int[] PageSizes { get; } = [10, 20];
    public bool IsSimple => Kind?.Kind is CatalogKind.Brands or CatalogKind.Categories or CatalogKind.Departments or CatalogKind.Specializations;
    public bool IsService => Kind?.Kind == CatalogKind.Services;
    public bool IsSupplier => Kind?.Kind == CatalogKind.Suppliers;
    public bool IsPart => Kind?.Kind == CatalogKind.Parts;
    public bool IsBay => Kind?.Kind == CatalogKind.Bays;
    public bool IsMechanic => Kind?.Kind == CatalogKind.Mechanics;
    public bool UsesPages => IsService || IsPart;
    public string PageText => TotalCount == 0 ? "Нет записей" : $"Страница {Page}, всего {TotalCount}";

    partial void OnKindChanged(CatalogOption? value)
    {
        NotifyShape();
        if (!_listsReady)
            return;
        NewItem();
        Page = 1;
        _ = LoadAsync();
    }

    partial void OnPageSizeChanged(int value)
    {
        if (value != 10 && value != 20)
            PageSize = 10;
        if (!_listsReady)
            return;
        Page = 1;
        _ = LoadAsync();
    }

    public override Task LoadAsync() => RunAsync(async () =>
    {
        NotifyShape();
        await Fill(Suppliers, () => _admin.GetSuppliersAsync());
        await Fill(Departments, () => _admin.GetDepartmentsAsync());
        await Fill(Specializations, () => _admin.GetSpecializationsAsync());
        if (ServiceChoices.Count == 0)
        {
            foreach (var service in await _admin.GetServicesLookupAsync())
                ServiceChoices.Add(new ServiceChoice { Id = service.Id, Name = service.Title });
        }

        var current = Kind ?? Kinds[0];
        IReadOnlyList<CatalogRow> rows;
        if (current.Kind == CatalogKind.Services)
        {
            decimal? max = InputValues.TryDecimal(MaxPrice, out var price) ? price : null;
            int? duration = InputValues.TryInt(MaxDuration, out var minutes) ? minutes : null;
            var page = await _admin.GetServicesAsync(Search, max, duration, Page, PageSize);
            TotalCount = page.Total;
            rows = page.Items.Select(x => new CatalogRow
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.BasePrice,
                Duration = x.DurationMinutes,
                Title = $"{x.Name} · {x.BasePrice:0.00} ₽, {x.DurationMinutes} мин"
            }).ToList();
        }
        else if (current.Kind == CatalogKind.Parts)
        {
            var page = await _admin.GetPartsAsync(Search, Supplier?.Id, BelowMinimum, Page, PageSize);
            TotalCount = page.Total;
            rows = page.Items.Select(x => new CatalogRow
            {
                Id = x.Id,
                Name = x.Name,
                Sku = x.Sku,
                SupplierId = x.SupplierId,
                PurchasePrice = x.PurchasePrice,
                Price = x.SalePrice,
                Quantity = x.Quantity,
                MinQuantity = x.MinQuantity,
                Title = $"{x.Name} · {x.Supplier} · ост. {x.Quantity}"
            }).ToList();
        }
        else
        {
            rows = await _admin.GetCatalogAsync(current.Kind);
            TotalCount = rows.Count;
        }

        Items.Clear();
        foreach (var row in rows)
            Items.Add(row);
        OnPropertyChanged(nameof(PageText));
        _listsReady = true;
    });

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (Page <= 1)
            return;
        Page--;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        var pages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Math.Max(1, PageSize)));
        if (Page >= pages)
            return;
        Page++;
        await LoadAsync();
    }

    partial void OnSelectedChanged(CatalogRow? value)
    {
        if (value is null)
            return;
        EditingId = value.Id;
        Name = value.Name;
        Description = value.Description;
        Phone = value.Phone;
        Email = value.Email;
        Sku = value.Sku;
        Login = value.Login;
        Password = "";
        Price = value.Price == 0 && !IsService && !IsPart ? "" : value.Price.ToString("0.##");
        PurchasePrice = value.PurchasePrice.ToString("0.##");
        Duration = value.Duration == 0 ? "" : value.Duration.ToString();
        Quantity = value.Quantity.ToString();
        MinQuantity = value.MinQuantity.ToString();
        Supplier = Suppliers.FirstOrDefault(x => x.Id == value.SupplierId);
        Department = Departments.FirstOrDefault(x => x.Id == value.DepartmentId);
        Specialization = Specializations.FirstOrDefault(x => x.Id == value.SpecializationId);
        BayKind = value.Kind;
        foreach (var choice in ServiceChoices)
            choice.IsSelected = value.ServiceIds.Contains(choice.Id);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private void NewItem()
    {
        Selected = null;
        EditingId = null;
        Name = Description = Phone = Email = Sku = Login = Password = Price = PurchasePrice = Duration = Quantity = MinQuantity = "";
        Supplier = null;
        Department = null;
        Specialization = null;
        BayKind = RepairBayKind.Подъёмник;
        foreach (var choice in ServiceChoices)
            choice.IsSelected = false;
        Error = "";
    }

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        if (Kind is null)
            return;
        switch (Kind.Kind)
        {
            case CatalogKind.Brands or CatalogKind.Categories or CatalogKind.Departments or CatalogKind.Specializations:
                await _admin.SaveNamedAsync(Kind.Kind, EditingId, Name);
                break;
            case CatalogKind.Services:
                if (!InputValues.TryDecimal(Price, out var price) || !InputValues.TryInt(Duration, out var duration))
                    throw new WorkshopException("Укажите стоимость и продолжительность целыми и дробными числами.");
                await _admin.SaveServiceAsync(EditingId, Name, Description, price, duration);
                break;
            case CatalogKind.Suppliers:
                await _admin.SaveSupplierAsync(EditingId, Name, Phone, Email);
                break;
            case CatalogKind.Parts:
                if (Supplier is null)
                    throw new WorkshopException("Выберите поставщика.");
                if (!InputValues.TryDecimal(PurchasePrice, out var purchase) || !InputValues.TryDecimal(Price, out var sale)
                    || !InputValues.TryInt(Quantity, out var qty) || !InputValues.TryInt(MinQuantity, out var min))
                    throw new WorkshopException("Проверьте цены и количества.");
                await _admin.SavePartAsync(EditingId, Supplier.Id, Name, Sku, purchase, sale, qty, min);
                break;
            case CatalogKind.Bays:
                await _admin.SaveBayAsync(EditingId, Name, BayKind);
                break;
            case CatalogKind.Mechanics:
                if (Department is null || Specialization is null)
                    throw new WorkshopException("Выберите отдел и специализацию.");
                await _admin.SaveMechanicAsync(
                    EditingId,
                    Name,
                    Login,
                    Password,
                    Department.Id,
                    Specialization.Id,
                    ServiceChoices.Where(x => x.IsSelected).Select(x => x.Id).ToArray());
                break;
        }

        await LoadAsync();
    });

    [RelayCommand]
    private Task DeleteAsync() => RunAsync(async () =>
    {
        if (Kind is null || EditingId is not int id)
            throw new WorkshopException("Выберите запись.");
        await _admin.DeleteNamedAsync(Kind.Kind, id);
        NewItem();
        await LoadAsync();
    });

    private void NotifyShape()
    {
        OnPropertyChanged(nameof(IsSimple));
        OnPropertyChanged(nameof(IsService));
        OnPropertyChanged(nameof(IsSupplier));
        OnPropertyChanged(nameof(IsPart));
        OnPropertyChanged(nameof(IsBay));
        OnPropertyChanged(nameof(IsMechanic));
        OnPropertyChanged(nameof(UsesPages));
    }

    private static async Task Fill(ObservableCollection<LookupItem> target, Func<Task<IReadOnlyList<LookupItem>>> load)
    {
        var items = await load();
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }
}
