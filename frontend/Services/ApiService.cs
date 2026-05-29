using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DataAnalizer.Models;
using System.Text.Json;

namespace DataAnalizer.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://127.0.0.1:8000";
        private string _jwtToken;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<UploadResponse> UploadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            using var content = new MultipartFormDataContent();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            content.Add(streamContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync("/upload", content);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<UploadResponse>())!;
        }

        public async Task<List<LogEntry>> GetLogsAsync()
        {
            var response = await _httpClient.GetAsync("/logs");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<LogEntry>>())!;
        }

        public async Task<AnalyticsData> GetLogDetailsAsync(int logId)
        {
            var response = await _httpClient.GetAsync($"/logs/{logId}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<AnalyticsData>())!;
        }

        public async Task<CityAnalytics?> GetCityDetailsAsync(int logId, string cityName)
        {
            var response = await _httpClient.GetAsync($"/city_details/{logId}/{cityName}");
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<CityAnalytics>())!;
            }
            return null;
        }

        // Poprawiona metoda logowania (wyciąga access_token)
        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new LoginRequest { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/login", payload);
            if (!response.IsSuccessStatusCode)
                return false;

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                
                // FastAPI domyślnie zwraca "access_token" zamiast "token"
                if (doc.RootElement.TryGetProperty("access_token", out var tokenElement))
                {
                    _jwtToken = tokenElement.GetString();
                    if (!string.IsNullOrEmpty(_jwtToken))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
                    }
                }
            }
            catch
            {
                // Ignoruj błędy parsowania; jeśli status był 200, to operacja się powiodła
            }

            return true;
        }

        // Poprawiona metoda rejestracji obsługująca 3 argumenty (w tym email)
        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            var payload = new RegisterRequest 
            { 
                Username = username, 
                Password = password, 
                Email = email 
            };
            
            var response = await _httpClient.PostAsJsonAsync("/register", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteLogAsync(int logId)
        {
            var response = await _httpClient.DeleteAsync($"/logs/{logId}");
            response.EnsureSuccessStatusCode();
        }
    }
}