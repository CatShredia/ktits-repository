using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Specialty
{
    public int IdSpecialty { get; set; }

    public string? Code { get; set; }

    public string? Direction { get; set; }

    public int? IdDepart { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Department? IdDepartNavigation { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
