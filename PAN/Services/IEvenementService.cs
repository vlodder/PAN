using PAN.Models;

namespace PAN.Services
{
    public interface IEvenementService
    {
        Task<List<EvenementListItem>> GetAllAsync();

        Task<List<EvenementListItem>> SearchAsync(string texte);

        Task<List<EvenementListItem>> SearchPagedAsync(
            string texte,
            string ville,
            int? idType,
            bool? estMouvant,
            int skip,
            int take);

        Task<List<string>> GetVillesAsync();

        Task<List<TypeOption>> GetTypesAsync();

        Task<List<EvenementListItem>> GetEventsForMapAsync(int skip, int take);

        Task<EvenementDetailItem?> GetByIdAsync(int idEvenement);

        Task<bool> AddUpVoteAsync(int idEvenement);
        Task<List<EvenementListItem>> GetLatestAsync(int take);
        Task<int> GetTotalCountAsync();
        Task<int> GetCityCountAsync();
    }

    public class TypeOption
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;
    }
}