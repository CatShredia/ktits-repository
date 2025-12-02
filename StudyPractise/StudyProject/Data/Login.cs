using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Login
{
    public int Id { get; set; }

    public string Login1 { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
