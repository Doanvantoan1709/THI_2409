using System;
using System.Collections.Generic;

namespace DETHI_2409.Entities;

public partial class Project
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
}
