using Avalonia.Controls;
using Avalonia.Interactivity;
using ProductionSystem.Client.Services;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class OrderPlanningWindow : Window
{
    public OrderPlanningWindow(BackendApi api, string orderNumber)
    {
        InitializeComponent();
        DataContext = new OrderPlanningViewModel(api, orderNumber);
    }

    private void Close_Click(object? sender, RoutedEventArgs e) => Close();
}
