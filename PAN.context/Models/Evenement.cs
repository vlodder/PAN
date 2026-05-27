using System;
using System.Collections.Generic;

namespace PAN.context.Models;

public partial class Evenement
{
    public int IdEvenement { get; set; }

    public DateTime? DateHeureObservation { get; set; }

    public string? Descriptif { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool? Estmouvant { get; set; }

    public int? UpVote { get; set; }

    public string? CompteRendu { get; set; }

    public int IdLocalisation { get; set; }

    public int? IdClassement { get; set; }

    public int? IdPhenomene { get; set; }

    public int IdType { get; set; }

    public int? IdUtilisateur { get; set; }

    public virtual Classement? IdClassementNavigation { get; set; }

    public virtual Localisation IdLocalisationNavigation { get; set; } = null!;

    public virtual Phenomene? IdPhenomeneNavigation { get; set; }

    public virtual Type IdTypeNavigation { get; set; } = null!;

    public virtual Utilisateur? IdUtilisateurNavigation { get; set; }
}
