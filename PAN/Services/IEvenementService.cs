using PAN.Models;

namespace PAN.Services
{
    public interface IEvenementService
    {
        Task<List<EvenementListItem>> GetAllAsync();
        Task<List<EvenementListItem>> SearchAsync(string texte);
    }
}