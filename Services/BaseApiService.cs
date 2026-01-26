using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Base API service for HTTP communications
    /// </summary>
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService()
        {
            // Use HttpClientHandler for better Windows MAUI localhost support
            var handler = new HttpClientHandler();
            
#if WINDOWS
            // On Windows, we need to handle localhost connections specially
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(Constants.ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(Constants.ApiTimeoutSeconds)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            SetAuthenticationHeader();
            
            System.Diagnostics.Debug.WriteLine($"[BaseApiService] Configured for: {Constants.ApiBaseUrl}");
        }

        /// <summary>
        /// Set authentication header with current token
        /// </summary>
        protected void SetAuthenticationHeader()
        {
            var token = Settings.AuthToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// GET request
        /// </summary>
        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                // Always refresh auth header in case token changed after login
                SetAuthenticationHeader();
                
                var response = await _httpClient.GetAsync(endpoint);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Sesión expirada");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// POST request
        /// </summary>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[PostAsync] Sending to: {_httpClient.BaseAddress}{endpoint}");
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
                
                System.Diagnostics.Debug.WriteLine($"[PostAsync] Response status: {response.StatusCode}");

                // For 401 Unauthorized, try to read the error response body (login failures)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PostAsync] Unauthorized response: {errorContent}");
                    
                    try
                    {
                        return JsonSerializer.Deserialize<TResponse>(errorContent, _jsonOptions);
                    }
                    catch
                    {
                        return default;
                    }
                }

                // For 400 Bad Request, try to read the error response body
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Try to read the response body which may contain error message
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PostAsync] BadRequest response: {errorContent}");
                    
                    // Try to deserialize from the string (since we already read the stream)
                    try
                    {
                        return JsonSerializer.Deserialize<TResponse>(errorContent, _jsonOptions);
                    }
                    catch
                    {
                        // If we can't deserialize, return null
                        return default;
                    }
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// PUT request
        /// </summary>
        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Sesión expirada");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// DELETE request
        /// </summary>
        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Sesión expirada");
                }

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Upload file with multipart form data
        /// </summary>
        protected async Task<TResponse?> UploadFileAsync<TResponse>(string endpoint, Stream fileStream, string fileName, Dictionary<string, string>? additionalData = null)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "file", fileName);

                if (additionalData != null)
                {
                    foreach (var kvp in additionalData)
                    {
                        content.Add(new StringContent(kvp.Value), kvp.Key);
                    }
                }

                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Sesión expirada");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
        }
    }
}
