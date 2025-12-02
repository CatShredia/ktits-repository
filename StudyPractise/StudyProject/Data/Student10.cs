using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Student10
{
    public int IdStudent { get; set; }

    public string Surname { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string School { get; set; } = null!;

    public double? Scores { get; set; }
}
