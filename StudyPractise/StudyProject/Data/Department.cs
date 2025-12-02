using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Department
{
    public int IdDepart { get; set; }

    public string? Cipher { get; set; }

    public string? NameDepart { get; set; }

    public int? IdFaculty { get; set; }

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Faculty? IdFacultyNavigation { get; set; }

    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}
