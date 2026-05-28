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
            return await BuildListQuery()
                .OrderByDescending(e => e.DateHeureObservation)
                .ToListAsync();
        }

        public async Task<List<EvenementListItem>> GetEventsForMapAsync(int skip, int take)
        {
            return await BuildListQuery()
                .Where(e => e.Latitude != null && e.Longitude != null)
                .OrderByDescending(e => e.DateHeureObservation)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<EvenementListItem>> SearchPagedAsync(
            string texte,
            string ville,
            int? idType,
            bool? estMouvant,
            int skip,
            int take)
        {
            var query = _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(texte))
            {
                var texteMin = texte.ToLower();

                query = query.Where(e =>
                    e.Descriptif != null &&
                    e.Descriptif.ToLower().Contains(texteMin));
            }

            if (!string.IsNullOrWhiteSpace(ville))
            {
                var villeMin = ville.ToLower();

                query = query.Where(e =>
                    e.IdLocalisationNavigation != null &&
                    e.IdLocalisationNavigation.Ville != null &&
                    e.IdLocalisationNavigation.Ville.ToLower() == villeMin);
            }

            if (idType.HasValue)
                query = query.Where(e => e.IdType == idType.Value);

            if (estMouvant.HasValue)
                query = query.Where(e => e.Estmouvant == estMouvant.Value);

            return await query
                .OrderByDescending(e => e.DateHeureObservation)
                .Skip(skip)
                .Take(take)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    DateHeureObservation = e.DateHeureObservation,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant,
                    UpVote = e.UpVote ?? 0,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,

                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,

                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty,

                    IdType = e.IdType
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
                    (e.Descriptif != null &&
                     e.Descriptif.ToLower().Contains(texte)) ||
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
                    DateHeureObservation = e.DateHeureObservation,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant,
                    UpVote = e.UpVote ?? 0,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,

                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,

                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty,

                    IdType = e.IdType
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetVillesAsync()
        {
            return await _context.Localisation
                .Where(l => l.Ville != null)
                .Select(l => l.Ville!)
                .Distinct()
                .OrderBy(v => v)
                .ToListAsync();
        }

        public async Task<List<TypeOption>> GetTypesAsync()
        {
            return await _context.Type
                .Select(t => new TypeOption
                {
                    Id = t.IdType,
                    Nom = t.Nom ?? string.Empty
                })
                .OrderBy(t => t.Nom)
                .ToListAsync();
        }

        public async Task<EvenementDetailItem?> GetByIdAsync(int idEvenement)
        {
            return await _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .Where(e => e.IdEvenement == idEvenement)
                .Select(e => new EvenementDetailItem
                {
                    IdEvenement = e.IdEvenement,
                    DateHeureObservation = e.DateHeureObservation,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant,
                    UpVote = e.UpVote ?? 0,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,

                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,

                    CodePostal = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.CodePostal
                        : null,

                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddUpVoteAsync(int idEvenement)
        {
            var entity = await _context.Evenement
                .FirstOrDefaultAsync(e => e.IdEvenement == idEvenement);

            if (entity == null)
                return false;

            entity.UpVote = (entity.UpVote ?? 0) + 1;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<EvenementListItem>> GetLatestAsync(int take)
        {
            return await BuildListQuery()
                .OrderByDescending(e => e.DateHeureObservation)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Evenement.CountAsync();
        }

        public async Task<int> GetCityCountAsync()
        {
            return await _context.Localisation
                .Where(l => l.Ville != null)
                .Select(l => l.Ville)
                .Distinct()
                .CountAsync();
        }
        private IQueryable<EvenementListItem> BuildListQuery()
        {
            return _context.Evenement
                .Include(e => e.IdLocalisationNavigation)
                .Include(e => e.IdTypeNavigation)
                .Select(e => new EvenementListItem
                {
                    IdEvenement = e.IdEvenement,
                    DateHeureObservation = e.DateHeureObservation,
                    Descriptif = e.Descriptif ?? string.Empty,
                    EstMouvant = e.Estmouvant,
                    UpVote = e.UpVote ?? 0,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,

                    Ville = e.IdLocalisationNavigation != null
                        ? e.IdLocalisationNavigation.Ville ?? string.Empty
                        : string.Empty,

                    TypeNom = e.IdTypeNavigation != null
                        ? e.IdTypeNavigation.Nom ?? string.Empty
                        : string.Empty,

                    IdType = e.IdType
                });
        }
    }
}