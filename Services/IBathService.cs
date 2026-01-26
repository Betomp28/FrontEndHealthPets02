using FrontEndHealthPets.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Services
{
    public interface IBathService
    {
        Task<List<BathRecord>> GetBathHistoryAsync(long petId);
        Task<bool> AddBathRecordAsync(BathRecord record);
        Task<bool> DeleteBathRecordAsync(int id);
    }
}
