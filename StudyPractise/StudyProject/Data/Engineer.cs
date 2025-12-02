using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Engineer
{
    public int TabNumber { get; set; }

    public string? Specialty { get; set; }

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
