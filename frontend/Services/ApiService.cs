using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DataAnalizer.Models;

namespace DataAnalizer.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://127.0.0.1:8000";

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

            return await response.Content.ReadFromJsonAsync<UploadResponse>();
        }

        public async Task<List<LogEntry>> GetLogsAsync()
        {
            var response = await _httpClient.GetAsync("/logs");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<LogEntry>>();
        }

        public async Task<AnalyticsData> GetLogDetailsAsync(int logId)
        {
            var response = await _httpClient.GetAsync($"/logs/{logId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AnalyticsData>();
        }

        public async Task<CityAnalytics> GetCityDetailsAsync(int logId, string cityName)
        {
            var response = await _httpClient.GetAsync($"/city_details/{logId}/{cityName}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CityAnalytics>();
            }
            return null;
        }

        public async Task DeleteLogAsync(int logId)
        {
            var response = await _httpClient.DeleteAsync($"/logs/{logId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
