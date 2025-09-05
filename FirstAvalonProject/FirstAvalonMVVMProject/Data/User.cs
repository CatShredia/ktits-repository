using System;
using System.Collections.Generic;

namespace FirstAvalonMVVMProject.Data;

public partial class User
{
    public int IdUser { get; set; }

    public string FirstName { get; set; } = null!;

    public string? SecondName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
