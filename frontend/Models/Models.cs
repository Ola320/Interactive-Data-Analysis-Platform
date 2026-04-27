using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAnalizer.Models
{
    public class LogEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }

    public class UploadResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("stats")]
        public AnalyticsData Stats { get; set; }
    }

    public class AnalyticsData
    {
        [JsonPropertyName("total_offers")]
        public int TotalOffers { get; set; }

        [JsonPropertyName("avg_price")]
        public double AvgPrice { get; set; }

        [JsonPropertyName("median_price")]
        public double MedianPrice { get; set; }

        [JsonPropertyName("avg_price_per_sqm")]
        public double AvgPricePerSqm { get; set; }

        [JsonPropertyName("top_cities")]
        public List<CityPrice> TopCities { get; set; }

        [JsonPropertyName("room_distribution")]
        public List<RoomDist> RoomDistribution { get; set; }

        [JsonPropertyName("price_vs_distance")]
        public List<PriceDistance> PriceVsDistance { get; set; }
    }

    public class CityPrice
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price_per_sqm")]
        public double PricePerSqm { get; set; }
    }

    public class RoomDist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class PriceDistance
    {
        [JsonPropertyName("distance")]
        public double Distance { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }
    }

    public class CityAnalytics
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("total_listings")]
        public int TotalListings { get; set; }

        [JsonPropertyName("avg_price")]
        public double AvgPrice { get; set; }

        [JsonPropertyName("avg_price_per_sqm")]
        public double AvgPricePerSqm { get; set; }
    }
}
