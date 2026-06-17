using DataAnalizer.Data;
using DataAnalizer.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAnalizer.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AnalysisRepository _analysisRepository;

        private const string BaseUrl = "http://127.0.0.1:8000";

        private string? _jwtToken;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            _analysisRepository = new AnalysisRepository();
        }

        public async Task<UploadResponse> UploadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "File not found",
                    filePath
                );
            }

            using var content = new MultipartFormDataContent();

            await using var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read
            );

            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue("text/csv");

            content.Add(
                streamContent,
                "file",
                Path.GetFileName(filePath)
            );

            HttpResponseMessage response =
                await _httpClient.PostAsync("/upload", content);

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Backend zwrócił błąd " +
                    $"{(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}:\n{responseJson}"
                );
            }

            
            using JsonDocument document =
                JsonDocument.Parse(responseJson);

            JsonElement root = document.RootElement;

            if (!root.TryGetProperty(
                    "id",
                    out JsonElement idElement))
            {
                throw new JsonException(
                    "Odpowiedź backendu nie zawiera pola 'id'."
                );
            }

            if (!root.TryGetProperty(
                    "stats",
                    out JsonElement statsElement))
            {
                throw new JsonException(
                    "Odpowiedź backendu nie zawiera pola 'stats'."
                );
            }

            int backendLogId = idElement.GetInt32();

            string analysisJson =
                statsElement.GetRawText();

           
            await _analysisRepository.SaveOrUpdateAsync(
                backendLogId,
                Path.GetFileName(filePath),
                DateTime.UtcNow,
                analysisJson
            );

            UploadResponse? uploadResponse =
                JsonSerializer.Deserialize<UploadResponse>(
                    responseJson,
                    JsonOptions
                );

            if (uploadResponse is null)
            {
                throw new JsonException(
                    "Nie udało się odczytać odpowiedzi po przesłaniu pliku."
                );
            }

            return uploadResponse;
        }

        public async Task<List<LogEntry>> GetLogsAsync()
        {
            var response = await _httpClient.GetAsync("/logs");

            response.EnsureSuccessStatusCode();

            return (
                await response.Content
                    .ReadFromJsonAsync<List<LogEntry>>()
            )!;
        }

        public async Task<AnalyticsData> GetLogDetailsAsync(int logId)
        {
           
            AnalysisRecord? savedRecord =
                await _analysisRepository
                    .GetByBackendIdAsync(logId);

            if (savedRecord is not null &&
                !string.IsNullOrWhiteSpace(savedRecord.AnalysisJson))
            {
                try
                {
                    AnalyticsData? savedAnalytics =
                        JsonSerializer.Deserialize<AnalyticsData>(
                            savedRecord.AnalysisJson,
                            JsonOptions
                        );

                    if (savedAnalytics is not null)
                    {
                        return savedAnalytics;
                    }
                }
                catch (JsonException)
                {
                    
                }
            }

           
            HttpResponseMessage response =
                await _httpClient.GetAsync($"/logs/{logId}");

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Backend zwrócił błąd " +
                    $"{(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}:\n{responseJson}"
                );
            }

            AnalyticsData? analytics =
                JsonSerializer.Deserialize<AnalyticsData>(
                    responseJson,
                    JsonOptions
                );

            if (analytics is null)
            {
                throw new JsonException(
                    $"Nie udało się odczytać analizy dla logu {logId}."
                );
            }

            
            await _analysisRepository.SaveOrUpdateAsync(
                logId,
                $"log_{logId}",
                DateTime.UtcNow,
                responseJson
            );

            return analytics;
        }

        public async Task<CityAnalytics?> GetCityDetailsAsync(
            int logId,
            string cityName,
            int? minRooms = null,
            int? maxRooms = null,
            double? minSqm = null,
            double? maxSqm = null,
            double? minPrice = null,
            double? maxPrice = null)
        {
            var url = $"/city_details/{logId}/{cityName}";

            var query = new List<string>();

            if (minRooms.HasValue)
                query.Add($"min_rooms={minRooms.Value}");

            if (maxRooms.HasValue)
                query.Add($"max_rooms={maxRooms.Value}");

            if (minSqm.HasValue)
                query.Add($"min_sqm={minSqm.Value}");

            if (maxSqm.HasValue)
                query.Add($"max_sqm={maxSqm.Value}");

            if (minPrice.HasValue)
                query.Add($"min_price={minPrice.Value}");

            if (maxPrice.HasValue)
                query.Add($"max_price={maxPrice.Value}");

            if (query.Count > 0)
            {
                url += "?" + string.Join("&", query);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return (
                    await response.Content
                        .ReadFromJsonAsync<CityAnalytics>()
                )!;
            }

            return null;
        }

        public async Task<bool> LoginAsync(
            string username,
            string password)
        {
            var payload = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "/login",
                    payload
                );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            try
            {
                var json =
                    await response.Content.ReadAsStringAsync();

                using var doc =
                    JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty(
                        "access_token",
                        out var tokenElement))
                {
                    _jwtToken =
                        tokenElement.GetString();

                    if (!string.IsNullOrEmpty(_jwtToken))
                    {
                        _httpClient
                            .DefaultRequestHeaders
                            .Authorization =
                            new AuthenticationHeaderValue(
                                "Bearer",
                                _jwtToken
                            );
                    }
                }
            }
            catch
            {
               
            }

            return true;
        }

        public async Task<bool> RegisterAsync(
            string username,
            string password,
            string email)
        {
            var payload = new RegisterRequest
            {
                Username = username,
                Password = password,
                Email = email
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "/register",
                    payload
                );

            return response.IsSuccessStatusCode;
        }

        public async Task DeleteLogAsync(int logId)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"/logs/{logId}"
                );

            response.EnsureSuccessStatusCode();

        
            await _analysisRepository.DeleteAsync(logId);
        }

        public async Task RenameLogAsync(int logId, string newName)
        {
            var payload = new
            {
                name = newName
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"/logs/{logId}/rename",
                payload
            );

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Backend zwrócił błąd " +
                    $"{(int)response.StatusCode}:\n{responseBody}"
                );
            }
        }

        // Pobieranie dostępnych miast dla danego logu
        public async Task<List<string>> GetAvailableCitiesAsync(
            int logId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"/cities/{logId}"
                );

            if (response.IsSuccessStatusCode)
            {
                return (
                    await response.Content
                        .ReadFromJsonAsync<List<string>>()
                ) ?? new List<string>();
            }

            return new List<string>();
        }

 
        public async Task<DeepAnalysisResponse?>
            GetDeepAnalysisAsync(
                DeepAnalysisRequest request)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "/deep_analysis",
                    request
                );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<DeepAnalysisResponse>();
            }

            return null;
        }

        public async Task<FilterRangesResponse?>
            GetFilterRangesAsync(int logId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"/filter_ranges/{logId}"
                );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<FilterRangesResponse>();
            }

            return null;
        }
    }
}