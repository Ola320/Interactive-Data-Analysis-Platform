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
        [JsonPropertyName("summary")]
        public SummaryData Summary { get; set; }

        [JsonPropertyName("charts")]
        public ChartData Charts { get; set; }

        [JsonPropertyName("scratter_points")]
        public List<ScatterPoint> ScatterPoints { get; set; }
    }

    public class SummaryData
    {
        [JsonPropertyName("total_offers")]
        public int TotalOffers { get; set; }

        [JsonPropertyName("avg_price")]
        public double AvgPrice { get; set; }

        [JsonPropertyName("median_price")]
        public double MedianPrice { get; set; }

        [JsonPropertyName("average_price_per_m^2")]
        public double AvgPricePerSqm { get; set; }
    }

    public class ChartData
    {
        [JsonPropertyName("city_chart")]
        public List<CityPrice> CityChart { get; set; }

        [JsonPropertyName("rooms_chart")]
        public List<RoomDist> RoomsChart { get; set; }

        [JsonPropertyName("price_vs_distance")]
        public List<PriceDistance> PriceVsDistance { get; set; }

        [JsonPropertyName("trends")]
        public List<TrendItem> Trends { get; set; }
    }

    public class CityPrice
    {
        [JsonPropertyName("city")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
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

        [JsonPropertyName("value")]
        public double Price { get; set; }
    }

    public class TrendItem
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("avg_price")]
        public double AvgPrice { get; set; }
    }

    public class ScatterPoint
    {
        [JsonPropertyName("squareMeters")]
        public double SquareMeters { get; set; }

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
