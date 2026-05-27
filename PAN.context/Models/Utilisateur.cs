using System;
using System.Collections.Generic;

namespace PAN.context.Models;

public partial class Utilisateur
{
    public int IdUtilisateur { get; set; }

    public string? Nom { get; set; }

    public bool IsGepan { get; set; }

    public virtual ICollection<Evenement> Evenement { get; set; } = new List<Evenement>();
}
