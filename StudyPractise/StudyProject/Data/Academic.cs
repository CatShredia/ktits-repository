using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Academic
{
    public int IdAcademics { get; set; }

    public string? Fullname { get; set; }

    public DateOnly? DateBirth { get; set; }

    public string? Specialization { get; set; }

    public int? YearTitle { get; set; }
}
