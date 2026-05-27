using Microsoft.EntityFrameworkCore;
using PAN.Models;

namespace PAN.Services
{
    public class EvenementService : IEvenementService
    {
        private readonly PAN.context.Models.GeipanContext _context;

        public EvenementService(PAN.context.Models.GeipanContext context)
        {
            _context = context;
        }

        public async Task<List<EvenementListItem>> GetAllAsync()
        {
            return await _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .OrderByDescending(e => e.DateHeureObservation)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    DateHeureObservation = e.DateHeureObservation ?? DateTime.MinValue,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant ?? false,
                    UpVote = e.UpVote ?? 0,
                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,
                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty
                })
                .ToListAsync();
        }

        public async Task<List<EvenementListItem>> SearchAsync(string texte)
        {
            var query = _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(texte))
            {
                texte = texte.ToLower();

                query = query.Where(e =>
                    (e.Descriptif != null && e.Descriptif.ToLower().Contains(texte)) ||
                    (e.IdLocalisationNavigation != null &&
                     e.IdLocalisationNavigation.Ville != null &&
                     e.IdLocalisationNavigation.Ville.ToLower().Contains(texte)) ||
                    (e.IdTypeNavigation != null &&
                     e.IdTypeNavigation.Nom != null &&
                     e.IdTypeNavigation.Nom.ToLower().Contains(texte)));
            }

            return await query
                .OrderByDescending(e => e.DateHeureObservation)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    DateHeureObservation = e.DateHeureObservation ?? DateTime.MinValue,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant ?? false,
                    UpVote = e.UpVote ?? 0,
                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,
                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty
                })
                .ToListAsync();
        }
    }
}