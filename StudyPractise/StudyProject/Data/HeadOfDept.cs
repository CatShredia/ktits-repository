using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class HeadOfDept
{
    public int TabNumber { get; set; }

    public int? ExperienceYears { get; set; }

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
