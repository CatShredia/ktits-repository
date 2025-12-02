using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Employee
{
    public int TabNumEmployee { get; set; }

    public int? IdDepart { get; set; }

    public string? Fullname { get; set; }

    public string? PositionEmp { get; set; }

    public decimal? Salary { get; set; }

    public int? Chief { get; set; }

    public virtual Employee? ChiefNavigation { get; set; }

    public virtual Engineer? Engineer { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual HeadOfDept? HeadOfDept { get; set; }

    public virtual Department? IdDepartNavigation { get; set; }

    public virtual ICollection<Employee> InverseChiefNavigation { get; set; } = new List<Employee>();

    public virtual Teacher? Teacher { get; set; }
}
