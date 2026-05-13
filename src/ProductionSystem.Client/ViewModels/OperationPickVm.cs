using CommunityToolkit.Mvvm.ComponentModel;

namespace ProductionSystem.Client.ViewModels;

public partial class OperationPickVm : ObservableObject
{
    public int Id { get; }
    public string Name { get; }

    [ObservableProperty] private bool _isSelected;

    public OperationPickVm(int id, string name, bool selected = false)
    {
        Id = id;
        Name = name;
        _isSelected = selected;
    }
}
