using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class EquipmentFailuresViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<EquipmentFailureDto> Failures { get; } = new();
    public ObservableCollection<EquipmentListItemDto> Equipment { get; } = new();

    [ObservableProperty] private EquipmentListItemDto? _selectedEquipment;
    [ObservableProperty] private DateTimeOffset? _startedAt = DatePickerValue.Today();
    [ObservableProperty] private string _reason = "";
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private EquipmentFailureDto? _selectedFailure;

    public EquipmentFailuresViewModel(BackendApi api)
    {
        _api = api;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await RefreshAsync();
        var eq = await _api.GetEquipmentAsync();
        Equipment.Clear();
        if (eq != null)
        {
            foreach (var e in eq)
                Equipment.Add(e);
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var list = await _api.GetEquipmentFailuresAsync();
        Failures.Clear();
        if (list != null)
        {
            foreach (var f in list)
                Failures.Add(f);
        }
    }

    [RelayCommand]
    public async Task RegisterFailureAsync()
    {
        if (SelectedEquipment is null || string.IsNullOrWhiteSpace(Reason))
        {
            StatusMessage = "Укажите оборудование и причину.";
            return;
        }

        var started = DatePickerValue.ToDateOnly(StartedAt) ?? DateOnly.FromDateTime(DateTime.Today);
        var (ok, err) = await _api.CreateEquipmentFailureAsync(
            SelectedEquipment.Marking,
            started.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))),
            Reason.Trim());
        StatusMessage = ok ? "Сбой зарегистрирован." : err;
        if (ok)
        {
            Reason = "";
            await RefreshAsync();
        }
    }

    [RelayCommand]
    public async Task EndFailureAsync()
    {
        if (SelectedFailure is null || SelectedFailure.EndedAt is not null)
            return;

        var (ok, err) = await _api.EndEquipmentFailureAsync(SelectedFailure.Id, DateTime.Now);
        StatusMessage = ok ? "Сбой закрыт." : err;
        if (ok)
            await RefreshAsync();
    }
}
