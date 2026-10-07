using System.Collections.ObjectModel;
using AutoService.AvaloniaApp.Input;
using AppSession = AutoService.AvaloniaApp.AppSession;
using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

public partial class AnalyticsViewModel : AppPageViewModel
{
    private readonly ChiefService _chief;

    public AnalyticsViewModel(ChiefService chief) => _chief = chief;

    public ObservableCollection<AnalyticsSection> Sections { get; } = [];

    [ObservableProperty] private string fromDate = "01.09.2026";
    [ObservableProperty] private string toDate = "31.12.2026";

    public override Task LoadAsync() => ShowAsync();

    [RelayCommand]
    private Task ShowAsync() => RunAsync(async () =>
    {
        if (!InputValues.TryDate(FromDate, out var from) || !InputValues.TryDate(ToDate, out var to))
            throw new WorkshopException("Период задаётся датами дд.мм.гггг.");
        var sections = await _chief.GetReportAsync(from, to);
        Sections.Clear();
        foreach (var section in sections)
            Sections.Add(section);
    });
}

public partial class ReviewsViewModel : AppPageViewModel
{
    private readonly AppSession _session;
    private readonly ReviewService _reviews;

    public ReviewsViewModel(AppSession session, ReviewService reviews)
    {
        _session = session;
        _reviews = reviews;
        Rating = 5;
        CreatedAt = InputValues.Format(DateTime.Now);
    }

    public ObservableCollection<ReviewRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Orders { get; } = [];
    public int[] Ratings { get; } = [1, 2, 3, 4, 5];

    public bool CanEdit => _session.User?.Role == RoleNames.Admin;

    [ObservableProperty] private LookupItem? order;
    [ObservableProperty] private int rating;
    [ObservableProperty] private string comment = "";
    [ObservableProperty] private string createdAt;

    public override Task LoadAsync() => RunAsync(async () =>
    {
        OnPropertyChanged(nameof(CanEdit));
        Items.Clear();
        foreach (var item in await _reviews.GetReviewsAsync())
            Items.Add(item);
        Orders.Clear();
        if (!CanEdit)
            return;
        foreach (var item in await _reviews.GetOrdersWithoutReviewAsync())
            Orders.Add(item);
    });

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        if (!CanEdit)
            throw new WorkshopException("Отзыв вводит администратор.");
        if (Order is null)
            throw new WorkshopException("Выберите завершённый заказ-наряд без отзыва.");
        if (!InputValues.TryDate(CreatedAt, out var created))
            throw new WorkshopException("Дата отзыва: дд.мм.гггг чч:мм.");
        await _reviews.SaveReviewAsync(Order.Id, Rating, Comment, created);
        Comment = "";
        await LoadAsync();
    });
}
