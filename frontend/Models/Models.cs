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
        public string Name { get; set; } = null!;

        [JsonPropertyName("date")]
        public string Date { get; set; } = null!;
    }

    public class UploadResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("stats")]
        public AnalyticsData Stats { get; set; } = null!;
    }

    public class AnalyticsData
    {
        [JsonPropertyName("summary")]
        public SummaryData Summary { get; set; } = null!;

        [JsonPropertyName("charts")]
        public ChartData Charts { get; set; } = null!;

        [JsonPropertyName("scratter_points")]
        public List<ScatterPoint> ScatterPoints { get; set; } = null!;
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
        public List<CityPrice> CityChart { get; set; } = null!;

        [JsonPropertyName("rooms_chart")]
        public List<RoomDist> RoomsChart { get; set; } = null!;

        [JsonPropertyName("price_vs_distance")]
        public List<PriceDistance> PriceVsDistance { get; set; } = null!;

        [JsonPropertyName("trends")]
        public List<TrendItem> Trends { get; set; } = null!;
    }

    public class CityPrice
    {
        [JsonPropertyName("city")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("value")]
        public double PricePerSqm { get; set; }
    }

    public class RoomDist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

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
        public string City { get; set; } = null!;

        [JsonPropertyName("total_listings")]
        public int TotalListings { get; set; }

        [JsonPropertyName("avg_price")]
        public double AvgPrice { get; set; }

        [JsonPropertyName("avg_price_per_sqm")]
        public double AvgPricePerSqm { get; set; }
    }
}
