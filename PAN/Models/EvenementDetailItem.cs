namespace PAN.Models
{
    public class EvenementDetailItem
    {
        public int IdEvenement { get; set; }
        public DateTime DateHeureObservation { get; set; }
        public string Descriptif { get; set; } = string.Empty;
        public bool EstMouvant { get; set; }
        public int UpVote { get; set; }

        public string Ville { get; set; } = string.Empty;
        public int? CodePostal { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string TypeNom { get; set; } = string.Empty;
    }
}