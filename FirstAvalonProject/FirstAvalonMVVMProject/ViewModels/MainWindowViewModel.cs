using System.Collections.Generic;
using System.Linq;
using FirstAvalonMVVMProject.Data;

namespace FirstAvalonMVVMProject.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public List<User> Users { get; set; }

    public MainWindowViewModel()
    {
        RefreshData();
    }
    
    public void RefreshData()
    {
        var usersFromDb = App.DbContext.Users.ToList();
        Users = usersFromDb;
        OnPropertyChanged(nameof(Users));
    }
}