using ProductionSystem.Client;

namespace ProductionSystem.Client.ViewModels;

public partial class RoleHomeViewModel : ViewModelBase
{
    public RoleHomeViewModel(string role)
    {
        Title = role switch
        {
            UserRoles.Customer => "Экран заказчика",
            UserRoles.Manager => "Экран менеджера",
            UserRoles.Designer => "Экран конструктора",
            UserRoles.Foreman => "Экран мастера",
            UserRoles.Director => "Экран директора",
            _ => "Главный экран",
        };
    }

    public string Title { get; }
}
