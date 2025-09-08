using System.Collections.Generic;
using System.Linq;
using FirstAvalonMVVMProject.Data;

namespace FirstAvalonMVVMProject.ViewModels;

public partial class LoginWindowViewModel : ViewModelBase
{
    public List<Login> Logins { get; set; }
    
    public LoginWindowViewModel()
    {
        RefreshData();
    }
    
    public void RefreshData()
    {
        var loginsFromDb = App.DbContext.Logins.ToList();
        Logins = loginsFromDb;
        OnPropertyChanged(nameof(Logins));
    }
}