using FrontEndHealthPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Services
{
    public class BathService : IBathService
    {
        // Mock storage - in a real app this would call API or DB
        private static List<BathRecord> _mockRecords = new List<BathRecord>();
        private static int _nextId = 1;

        public async Task<List<BathRecord>> GetBathHistoryAsync(long petId)
        {
            // Simulate API delay
            await Task.Delay(100);
            return _mockRecords.Where(r => r.PetId == petId).OrderByDescending(r => r.Date).ToList();
        }

        public async Task<bool> AddBathRecordAsync(BathRecord record)
        {
            await Task.Delay(100);
            record.Id = _nextId++;
            _mockRecords.Add(record);
            return true;
        }

        public async Task<bool> DeleteBathRecordAsync(int id)
        {
            await Task.Delay(100);
            var record = _mockRecords.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                _mockRecords.Remove(record);
                return true;
            }
            return false;
        }
    }
}
