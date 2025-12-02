using System;
using System.Collections.Generic;

namespace StudyProject.Data;

public partial class Application
{
    public int IdApp { get; set; }

    public int? IdSpecialty { get; set; }

    public int? IdDisc { get; set; }

    public virtual Discipline? IdDiscNavigation { get; set; }

    public virtual Specialty? IdSpecialtyNavigation { get; set; }
}
