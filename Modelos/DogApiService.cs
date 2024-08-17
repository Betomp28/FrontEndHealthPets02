using FrontEndHealthPets.Entidades.response;
using FrontEndHealthPets.Entidades.Response;
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

        public async Task<Res_ObtenerBreeds> GetBreedsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://dog.ceo/api/breeds/list/all");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("JSON Response: " + json); // Debugging

                // Deserializar en la clase intermedia
                var breedResponse = JsonConvert.DeserializeObject<BreedResponse>(json);

                // Mapear a Res_ObtenerBreeds
                return new Res_ObtenerBreeds
                {
                    Breeds = breedResponse?.Message ?? new Dictionary<string, List<string>>(),
                    Status = breedResponse?.Status
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Res_ObtenerImages> GetImagesAsync(string breed)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://dog.ceo/api/breed/{breed}/images/random");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("JSON Response: " + json); // Debugging

                var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(json);

                return new Res_ObtenerImages
                {
                    Images = imageResponse?.Message,
                    Status = imageResponse?.Status
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}