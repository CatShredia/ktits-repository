using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Faculty
{
    public int IdFaculty { get; set; }

    public string? Abbreviation { get; set; }

    public string? NameFaculty { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
