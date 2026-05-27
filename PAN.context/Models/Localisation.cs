using System;
using System.Collections.Generic;

namespace PAN.context.Models;

public partial class Localisation
{
    public int IdLocalisation { get; set; }

    public int? CodePostal { get; set; }

    public string? Ville { get; set; }

    public virtual ICollection<Evenement> Evenement { get; set; } = new List<Evenement>();
}
