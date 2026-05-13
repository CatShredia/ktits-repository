using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class WorkersViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<WorkerListItemDto> Workers { get; } = new();
    public ObservableCollection<OperationPickVm> Operations { get; } = new();

    [ObservableProperty] private WorkerListItemDto? _selectedWorker;
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private string _firstMiddleName = "";
    [ObservableProperty] private DateTime? _birthDate = new DateTime(1990, 1, 1);
    [ObservableProperty] private string _homeAddress = "";
    [ObservableProperty] private string _education = "";
    [ObservableProperty] private string _qualification = "";
    [ObservableProperty] private string _newOperationName = "";
    [ObservableProperty] private bool _isCreatingNew;

    public WorkersViewModel(BackendApi api)
    {
        _api = api;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await RefreshOperationsAsync();
        await RefreshWorkersAsync();
    }

    partial void OnSelectedWorkerChanged(WorkerListItemDto? value)
    {
        if (value != null)
            IsCreatingNew = false;

        _ = LoadSelectedAsync();
    }

    [RelayCommand]
    public async Task RefreshWorkersAsync()
    {
        var list = await _api.GetWorkersAsync();
        Workers.Clear();
        if (list == null)
            return;

        foreach (var w in list)
            Workers.Add(w);
    }

    private async Task RefreshOperationsAsync()
    {
        var list = await _api.GetOperationsAsync();
        Operations.Clear();
        if (list == null)
            return;

        foreach (var o in list)
            Operations.Add(new OperationPickVm(o.Id, o.Name));
    }

    private async Task LoadSelectedAsync()
    {
        if (IsCreatingNew)
            return;

        if (SelectedWorker == null)
            return;

        var detail = await _api.GetWorkerAsync(SelectedWorker.Id);
        if (detail == null)
            return;

        LastName = detail.LastName;
        FirstMiddleName = detail.FirstMiddleName;
        BirthDate = detail.BirthDate.ToDateTime(TimeOnly.MinValue);
        HomeAddress = detail.HomeAddress;
        Education = detail.Education;
        Qualification = detail.Qualification;

        foreach (var op in Operations)
            op.IsSelected = detail.OperationIds.Contains(op.Id);
    }

    [RelayCommand]
    private void StartNew()
    {
        IsCreatingNew = true;
        SelectedWorker = null;
        LastName = "";
        FirstMiddleName = "";
        BirthDate = new DateTime(1990, 1, 1);
        HomeAddress = "";
        Education = "";
        Qualification = "";
        foreach (var op in Operations)
            op.IsSelected = false;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner == null)
            return;

        if (string.IsNullOrWhiteSpace(LastName))
        {
            await DialogService.ShowInfoAsync(owner, "Укажите фамилию и дату рождения.");
            return;
        }

        var body = new WorkerCreateUpdateRequest
        {
            LastName = LastName.Trim(),
            FirstMiddleName = FirstMiddleName.Trim(),
            BirthDate = DateOnly.FromDateTime(BirthDate ?? DateTime.Today),
            HomeAddress = HomeAddress.Trim(),
            Education = Education.Trim(),
            Qualification = Qualification.Trim(),
            OperationIds = Operations.Where(o => o.IsSelected).Select(o => o.Id).ToList(),
        };

        if (IsCreatingNew)
        {
            var (ok, err) = await _api.CreateWorkerAsync(body);
            if (!ok)
            {
                await DialogService.ShowInfoAsync(owner, err ?? "Ошибка сохранения.");
                return;
            }

            IsCreatingNew = false;
        }
        else if (SelectedWorker != null)
        {
            var (ok, err) = await _api.UpdateWorkerAsync(SelectedWorker.Id, body);
            if (!ok)
            {
                await DialogService.ShowInfoAsync(owner, err ?? "Ошибка сохранения.");
                return;
            }
        }
        else
        {
            await DialogService.ShowInfoAsync(owner, "Выберите работника или создайте нового.");
            return;
        }

        await RefreshWorkersAsync();
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (SelectedWorker == null || IsCreatingNew)
            return;

        var owner = DialogService.TryGetMainWindow();
        if (owner == null)
            return;

        if (!await DialogService.ConfirmAsync(owner, "Удалить выбранного работника?"))
            return;

        var (ok, err) = await _api.DeleteWorkerAsync(SelectedWorker.Id);
        if (!ok)
        {
            await DialogService.ShowInfoAsync(owner, err ?? "Ошибка удаления.");
            return;
        }

        StartNew();
        await RefreshWorkersAsync();
    }

    [RelayCommand]
    public async Task AddOperationAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner == null)
            return;

        if (string.IsNullOrWhiteSpace(NewOperationName))
        {
            await DialogService.ShowInfoAsync(owner, "Введите название операции.");
            return;
        }

        var (ok, err) = await _api.CreateOperationAsync(NewOperationName.Trim());
        if (!ok)
        {
            await DialogService.ShowInfoAsync(owner, err ?? "Не удалось добавить операцию.");
            return;
        }

        NewOperationName = "";
        await RefreshOperationsAsync();
        if (SelectedWorker != null)
            await LoadSelectedAsync();
    }
}
