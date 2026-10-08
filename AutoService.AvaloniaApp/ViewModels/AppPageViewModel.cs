using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

// Общая модель раздела: загрузка и текст ошибки.
// Базовый класс экранов в папке ViewModels.
public abstract partial class AppPageViewModel : ObservableObject
{
    [ObservableProperty] private string error = "";
    [ObservableProperty] private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(Error);

    public abstract Task LoadAsync();

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(HasError));

    protected async Task RunAsync(Func<Task> action)
    {
        try
        {
            Error = "";
            IsBusy = true;
            await action();
        }
        catch (WorkshopException ex)
        {
            Error = ex.Message;
        }
        catch (Exception ex)
        {
            Error = "Не удалось выполнить операцию. " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// Раздел со страницами по 10 или 20 строк.
// Базовый класс списков клиентов, автомобилей, записей и заказов.
public abstract partial class PagedViewModel : AppPageViewModel
{
    [ObservableProperty] private int page = 1;
    [ObservableProperty] private int pageSize = 10;
    [ObservableProperty] private int totalCount;
    private bool _ready;

    public int[] PageSizes { get; } = [10, 20];

    public string PageText
    {
        get
        {
            if (TotalCount == 0)
                return "Нет записей";
            var pages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Math.Max(1, PageSize)));
            return $"Страница {Page} из {pages}, всего {TotalCount}";
        }
    }

    protected void MarkReady() => _ready = true;

    partial void OnPageChanged(int value) => OnPropertyChanged(nameof(PageText));
    partial void OnTotalCountChanged(int value) => OnPropertyChanged(nameof(PageText));

    partial void OnPageSizeChanged(int value)
    {
        if (value != 10 && value != 20)
            PageSize = 10;
        OnPropertyChanged(nameof(PageText));
        if (!_ready)
            return;
        Page = 1;
        _ = LoadAsync();
    }

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

    protected void ApplyPage<T>(PageResult<T> result, Action<IReadOnlyList<T>> fill)
    {
        TotalCount = result.Total;
        fill(result.Items);
        OnPropertyChanged(nameof(PageText));
    }
}
