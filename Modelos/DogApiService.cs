using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Modelo
{
    class DogApiService
    {
        private readonly HttpClient _httpClient;

        public DogApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, List<string>>> GetBreedsAsync()
        {
            var response = await _httpClient.GetStringAsync("api/Perros/GetBreeds");
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(response);
        }

        public async Task<List<string>> GetImagesAsync(string breed)
        {
            var response = await _httpClient.GetStringAsync($"api/Perros/GetImages/{breed}");
            // Aquí deberías definir el tipo esperado para la respuesta
            return JsonConvert.DeserializeObject<List<string>>(response);
        }
    }
}