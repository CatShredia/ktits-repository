using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Teacher
{
    public int TabNumber { get; set; }

    public string? Rank { get; set; }

    public string? Degree { get; set; }

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
