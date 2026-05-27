using System;
using System.Collections.Generic;

namespace PAN.context.Models;

public partial class Classement
{
    public int IdClassement { get; set; }

    public string? Nom { get; set; }

    public virtual ICollection<Evenement> Evenement { get; set; } = new List<Evenement>();
}
