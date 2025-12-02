using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Discipline
{
    public int IdDiscipline { get; set; }

    public int? Code { get; set; }

    public int? Hours { get; set; }

    public string? NameDisc { get; set; }

    public int? IdDepart { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual Department? IdDepartNavigation { get; set; }
}
