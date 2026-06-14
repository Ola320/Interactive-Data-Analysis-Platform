using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8618 // Wyłączenie ostrzeżeń o braku inicjalizacji pól nie-null (przydatne przy modelach DTO)

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

    public class RegisterRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } // Dodane pole Email dopasowane do FastAPI

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    public class AnalysisFilters
    {
        // 1. DANE KATEGORIALNE (Listy wybranych elementów z "Select All")
        [JsonPropertyName("cities")] public List<string> Cities { get; set; } = new();
        [JsonPropertyName("types")] public List<string> Types { get; set; } = new();
        [JsonPropertyName("ownerships")] public List<string> Ownerships { get; set; } = new();
        [JsonPropertyName("building_materials")] public List<string> BuildingMaterials { get; set; } = new();
        [JsonPropertyName("conditions")] public List<string> Conditions { get; set; } = new();

        // 2. DANE NUMERYCZNE GŁÓWNE (Min/Max)
        [JsonPropertyName("min_price")] public double? MinPrice { get; set; }
        [JsonPropertyName("max_price")] public double? MaxPrice { get; set; }
        [JsonPropertyName("min_sqm")] public double? MinSqm { get; set; }
        [JsonPropertyName("max_sqm")] public double? MaxSqm { get; set; }
        [JsonPropertyName("min_rooms")] public double? MinRooms { get; set; }
        [JsonPropertyName("max_rooms")] public double? MaxRooms { get; set; }
        [JsonPropertyName("min_floor")] public double? MinFloor { get; set; }
        [JsonPropertyName("max_floor")] public double? MaxFloor { get; set; }
        [JsonPropertyName("min_floor_count")] public double? MinFloorCount { get; set; }
        [JsonPropertyName("max_floor_count")] public double? MaxFloorCount { get; set; }
        [JsonPropertyName("min_build_year")] public double? MinBuildYear { get; set; }
        [JsonPropertyName("max_build_year")] public double? MaxBuildYear { get; set; }

        // 3. DANE GEOGRAFICZNE (Min/Max)
        [JsonPropertyName("min_latitude")] public double? MinLatitude { get; set; }
        [JsonPropertyName("max_latitude")] public double? MaxLatitude { get; set; }
        [JsonPropertyName("min_longitude")] public double? MinLongitude { get; set; }
        [JsonPropertyName("max_longitude")] public double? MaxLongitude { get; set; }

        // 4. INFRASTRUKTURA I POI (Min/Max)
        [JsonPropertyName("min_centre_distance")] public double? MinCentreDistance { get; set; }
        [JsonPropertyName("max_centre_distance")] public double? MaxCentreDistance { get; set; }
        [JsonPropertyName("min_poi")] public double? MinPoiCount { get; set; }
        [JsonPropertyName("max_poi")] public double? MaxPoiCount { get; set; }

        [JsonPropertyName("min_school_dist")] public double? MinSchoolDist { get; set; }
        [JsonPropertyName("max_school_dist")] public double? MaxSchoolDist { get; set; }
        [JsonPropertyName("min_clinic_dist")] public double? MinClinicDist { get; set; }
        [JsonPropertyName("max_clinic_dist")] public double? MaxClinicDist { get; set; }
        [JsonPropertyName("min_post_office_dist")] public double? MinPostOfficeDist { get; set; }
        [JsonPropertyName("max_post_office_dist")] public double? MaxPostOfficeDist { get; set; }
        [JsonPropertyName("min_kindergarten_dist")] public double? MinKindergartenDist { get; set; }
        [JsonPropertyName("max_kindergarten_dist")] public double? MaxKindergartenDist { get; set; }
        [JsonPropertyName("min_restaurant_dist")] public double? MinRestaurantDist { get; set; }
        [JsonPropertyName("max_restaurant_dist")] public double? MaxRestaurantDist { get; set; }
        [JsonPropertyName("min_college_dist")] public double? MinCollegeDist { get; set; }
        [JsonPropertyName("max_college_dist")] public double? MaxCollegeDist { get; set; }
        [JsonPropertyName("min_pharmacy_dist")] public double? MinPharmacyDist { get; set; }
        [JsonPropertyName("max_pharmacy_dist")] public double? MaxPharmacyDist { get; set; }

        // 5. UDOGODNIENIA (Boolean: Tak/Nie/Obojętnie)
        [JsonPropertyName("has_parking")] public bool? HasParkingSpace { get; set; }
        [JsonPropertyName("has_balcony")] public bool? HasBalcony { get; set; }
        [JsonPropertyName("has_elevator")] public bool? HasElevator { get; set; }
        [JsonPropertyName("has_security")] public bool? HasSecurity { get; set; }
        [JsonPropertyName("has_storage_room")] public bool? HasStorageRoom { get; set; }
    }

    public class DeepAnalysisRequest
    {
        [JsonPropertyName("log_id")] public int LogId { get; set; }
        [JsonPropertyName("filters")] public AnalysisFilters Filters { get; set; } = new();

        // --- OPCJE WYNIKÓW (Czego oczekuje użytkownik) ---
        [JsonPropertyName("requested_kpis")] public List<string> RequestedKpis { get; set; } = new();
        [JsonPropertyName("requested_charts")] public List<int> RequestedCharts { get; set; } = new();
    }

    public class DeepAnalysisResponse
    {
        [JsonPropertyName("kpis")] public AnalysisKpis Kpis { get; set; } = new();
    }

    public class ChartDataFormat
    {
        [JsonPropertyName("labels")] public List<string> Labels { get; set; } = new();
        [JsonPropertyName("values")] public List<double> Values { get; set; } = new();
    }

    public class ScatterPointData
    {
        [JsonPropertyName("x")] public double X { get; set; }
        [JsonPropertyName("y")] public double Y { get; set; }
    }

    public class ChartsResponse
    {
        [JsonPropertyName("chart1")] public ChartDataFormat Chart1 { get; set; }
        [JsonPropertyName("chart2")] public ChartDataFormat Chart2 { get; set; }
        [JsonPropertyName("chart3")] public ChartDataFormat Chart3 { get; set; }
        [JsonPropertyName("chart4")] public List<ScatterPointData> Chart4 { get; set; }
        [JsonPropertyName("chart5")] public ChartDataFormat Chart5 { get; set; }
    }

    public class AnalysisKpis
    {
        [JsonPropertyName("count")] public int? Count { get; set; }
        [JsonPropertyName("market_share")] public double? MarketShare { get; set; }
        [JsonPropertyName("avg_price")] public double? AvgPrice { get; set; }
        [JsonPropertyName("avg_price_sqm")] public double? AvgPriceSqm { get; set; }
        [JsonPropertyName("avg_building_age")] public double? AvgBuildingAge { get; set; }

        // Odległości
        [JsonPropertyName("avg_centre_distance")] public double? AvgCentreDistance { get; set; }
        [JsonPropertyName("avg_school_dist")] public double? AvgSchoolDist { get; set; }
        [JsonPropertyName("avg_clinic_dist")] public double? AvgClinicDist { get; set; }
        [JsonPropertyName("avg_post_office_dist")] public double? AvgPostOfficeDist { get; set; }
        [JsonPropertyName("avg_kindergarten_dist")] public double? AvgKindergartenDist { get; set; }
        [JsonPropertyName("avg_restaurant_dist")] public double? AvgRestaurantDist { get; set; }
        [JsonPropertyName("avg_college_dist")] public double? AvgCollegeDist { get; set; }
        [JsonPropertyName("avg_pharmacy_dist")] public double? AvgPharmacyDist { get; set; }

        // Wartości Udogodnień
        [JsonPropertyName("cost_parking")] public double? CostParking { get; set; }
        [JsonPropertyName("cost_balcony")] public double? CostBalcony { get; set; }
        [JsonPropertyName("cost_elevator")] public double? CostElevator { get; set; }
        [JsonPropertyName("cost_security")] public double? CostSecurity { get; set; }
        [JsonPropertyName("cost_storage")] public double? CostStorage { get; set; }
    }

    public class MinMaxRange
    {
        [JsonPropertyName("min")] public double Min { get; set; }
        [JsonPropertyName("max")] public double Max { get; set; }
    }

    public class CategoryRanges
    {
        [JsonPropertyName("cities")] public List<string> Cities { get; set; } = new();
        [JsonPropertyName("types")] public List<string> Types { get; set; } = new();
        [JsonPropertyName("materials")] public List<string> Materials { get; set; } = new();
        [JsonPropertyName("conditions")] public List<string> Conditions { get; set; } = new();
        [JsonPropertyName("ownerships")] public List<string> Ownerships { get; set; } = new();
    }

    public class NumericRanges
    {
        [JsonPropertyName("price")] public MinMaxRange Price { get; set; } = new();
        [JsonPropertyName("sqm")] public MinMaxRange Sqm { get; set; } = new();
        [JsonPropertyName("rooms")] public MinMaxRange Rooms { get; set; } = new();
        [JsonPropertyName("floor")] public MinMaxRange Floor { get; set; } = new();
        [JsonPropertyName("floorCount")] public MinMaxRange FloorCount { get; set; } = new();
        [JsonPropertyName("buildYear")] public MinMaxRange BuildYear { get; set; } = new();
        [JsonPropertyName("centreDistance")] public MinMaxRange CentreDistance { get; set; } = new();
        [JsonPropertyName("poiCount")] public MinMaxRange PoiCount { get; set; } = new();
        [JsonPropertyName("schoolDistance")] public MinMaxRange SchoolDistance { get; set; } = new();
        [JsonPropertyName("pharmacyDistance")] public MinMaxRange PharmacyDistance { get; set; } = new();
        [JsonPropertyName("clinicDistance")] public MinMaxRange ClinicDistance { get; set; } = new();
        [JsonPropertyName("postOfficeDistance")] public MinMaxRange PostOfficeDistance { get; set; } = new();
        [JsonPropertyName("kindergartenDistance")] public MinMaxRange KindergartenDistance { get; set; } = new();
        [JsonPropertyName("restaurantDistance")] public MinMaxRange RestaurantDistance { get; set; } = new();
        [JsonPropertyName("collegeDistance")] public MinMaxRange CollegeDistance { get; set; } = new();
    }

    public class FilterRangesResponse
    {
        [JsonPropertyName("categories")] public CategoryRanges Categories { get; set; } = new();
        [JsonPropertyName("numeric")] public NumericRanges Numeric { get; set; } = new();
    }

    public static class AppState
    {
        // Domyślnie ustawiamy na 1
        public static int CurrentLogId { get; set; } = 1;
    }
}