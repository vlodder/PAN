using System;
using System.Collections.Generic;

namespace PAN.context.Models;

public partial class Type
{
    public int IdType { get; set; }

    public string? Nom { get; set; }

    public virtual ICollection<Evenement> Evenement { get; set; } = new List<Evenement>();
}
