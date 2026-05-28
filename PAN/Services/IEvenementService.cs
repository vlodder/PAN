using PAN.Models;

namespace PAN.Services
{
    public interface IEvenementService
    {
        Task<List<EvenementListItem>> SearchPagedAsync(
            string texte,
            string ville,
            int? idType,
            bool? estMouvant,
            int skip,
            int take);

        Task<List<string>> GetVillesAsync();
        Task<List<TypeOption>> GetTypesAsync();

        Task<EvenementDetailItem?> GetByIdAsync(int idEvenement);
        Task<bool> AddUpVoteAsync(int idEvenement);
    }

    public class TypeOption
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
    }
}