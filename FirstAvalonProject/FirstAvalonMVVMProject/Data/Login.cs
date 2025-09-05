using System;
using System.Collections.Generic;

namespace FirstAvalonMVVMProject.Data;

public partial class Login
{
    public int IdLogin { get; set; }

    public string Login1 { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
