using System;
using System.Collections.Generic;

namespace TaskSphere.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string Status { get; set; } = null!;

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
