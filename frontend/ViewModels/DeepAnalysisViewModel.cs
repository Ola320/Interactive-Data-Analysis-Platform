using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DataAnalizer.Models;
using DataAnalizer.Services;
using System.Collections.Generic;

namespace DataAnalizer.ViewModels
{
    public class SelectionItem : INotifyPropertyChanged
    {
        private bool _isSelected = true;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class FilterGroup : INotifyPropertyChanged
    {
        public ObservableCollection<SelectionItem> Items { get; set; } = new();
        private bool _isAllSelected = true;
        private bool _isUpdating = false;

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value; OnPropertyChanged();
                    if (!_isUpdating) { _isUpdating = true; foreach (var item in Items) item.IsSelected = value; _isUpdating = false; }
                }
            }
        }
        public void AddItem(string name) { var item = new SelectionItem { Name = name }; item.PropertyChanged += (s, e) => CheckIfAllSelected(); Items.Add(item); }
        private void CheckIfAllSelected() { if (_isUpdating) return; _isUpdating = true; IsAllSelected = Items.All(x => x.IsSelected); _isUpdating = false; }
        public List<string> GetSelectedNames() => Items.Where(x => x.IsSelected).Select(x => x.Name).ToList();
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class DeepAnalysisViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new ApiService();

        // 1. FILTRY KATEGORIALNE
        public FilterGroup CitiesGroup { get; set; } = new();
        public FilterGroup TypesGroup { get; set; } = new();
        public FilterGroup OwnershipsGroup { get; set; } = new();
        public FilterGroup MaterialsGroup { get; set; } = new();
        public FilterGroup ConditionsGroup { get; set; } = new();

        // 2. ETYKIETY Z PODPOWIEDZIAMI WIDEŁEK (Bindowane)
        private string _lblPrice = "Cena:", _lblSqm = "Metraż:", _lblRooms = "Pokoje:", _lblFloor = "Piętro:", _lblFloorCount = "Pięter:", _lblYear = "Rok Budowy:", _lblDist = "Do Centrum:", _lblPoi = "Ilość POI:", _lblSchool = "Do Szkoły:", _lblPharm = "Do Apteki:";
        private string _lblClinic = "Do Przychodni:", _lblPost = "Do Poczty:", _lblKinder = "Do Przedszkola:", _lblRest = "Do Restauracji:", _lblCol = "Do Uczelni:";

        public string LblPrice { get => _lblPrice; set { _lblPrice = value; OnPropertyChanged(); } }
        public string LblSqm { get => _lblSqm; set { _lblSqm = value; OnPropertyChanged(); } }
        public string LblRooms { get => _lblRooms; set { _lblRooms = value; OnPropertyChanged(); } }
        public string LblFloor { get => _lblFloor; set { _lblFloor = value; OnPropertyChanged(); } }
        public string LblFloorCount { get => _lblFloorCount; set { _lblFloorCount = value; OnPropertyChanged(); } }
        public string LblYear { get => _lblYear; set { _lblYear = value; OnPropertyChanged(); } }
        public string LblDist { get => _lblDist; set { _lblDist = value; OnPropertyChanged(); } }
        public string LblPoi { get => _lblPoi; set { _lblPoi = value; OnPropertyChanged(); } }
        public string LblSchool { get => _lblSchool; set { _lblSchool = value; OnPropertyChanged(); } }
        public string LblPharm { get => _lblPharm; set { _lblPharm = value; OnPropertyChanged(); } }
        public string LblClinic { get => _lblClinic; set { _lblClinic = value; OnPropertyChanged(); } }
        public string LblPost { get => _lblPost; set { _lblPost = value; OnPropertyChanged(); } }
        public string LblKinder { get => _lblKinder; set { _lblKinder = value; OnPropertyChanged(); } }
        public string LblRest { get => _lblRest; set { _lblRest = value; OnPropertyChanged(); } }
        public string LblCol { get => _lblCol; set { _lblCol = value; OnPropertyChanged(); } }

        // 3. WARTOŚCI WPISANE (NUMERYCZNE)
        private string _minPrice = "", _maxPrice = "", _minSqm = "", _maxSqm = "", _minRooms = "", _maxRooms = "";
        private string _minFloor = "", _maxFloor = "", _minFloorCount = "", _maxFloorCount = "", _minYear = "", _maxYear = "";
        private string _minDist = "", _maxDist = "", _minPoi = "", _maxPoi = "", _minSchool = "", _maxSchool = "", _minPharm = "", _maxPharm = "";
        private string _minClinic = "", _maxClinic = "", _minPost = "", _maxPost = "", _minKinder = "", _maxKinder = "", _minRest = "", _maxRest = "", _minCol = "", _maxCol = "";

        public string MinPrice { get => _minPrice; set { _minPrice = value; OnPropertyChanged(); } }
        public string MaxPrice { get => _maxPrice; set { _maxPrice = value; OnPropertyChanged(); } }
        public string MinSqm { get => _minSqm; set { _minSqm = value; OnPropertyChanged(); } }
        public string MaxSqm { get => _maxSqm; set { _maxSqm = value; OnPropertyChanged(); } }
        public string MinRooms { get => _minRooms; set { _minRooms = value; OnPropertyChanged(); } }
        public string MaxRooms { get => _maxRooms; set { _maxRooms = value; OnPropertyChanged(); } }
        public string MinFloor { get => _minFloor; set { _minFloor = value; OnPropertyChanged(); } }
        public string MaxFloor { get => _maxFloor; set { _maxFloor = value; OnPropertyChanged(); } }
        public string MinFloorCount { get => _minFloorCount; set { _minFloorCount = value; OnPropertyChanged(); } }
        public string MaxFloorCount { get => _maxFloorCount; set { _maxFloorCount = value; OnPropertyChanged(); } }
        public string MinYear { get => _minYear; set { _minYear = value; OnPropertyChanged(); } }
        public string MaxYear { get => _maxYear; set { _maxYear = value; OnPropertyChanged(); } }
        public string MinDist { get => _minDist; set { _minDist = value; OnPropertyChanged(); } }
        public string MaxDist { get => _maxDist; set { _maxDist = value; OnPropertyChanged(); } }
        public string MinPoi { get => _minPoi; set { _minPoi = value; OnPropertyChanged(); } }
        public string MaxPoi { get => _maxPoi; set { _maxPoi = value; OnPropertyChanged(); } }
        public string MinSchool { get => _minSchool; set { _minSchool = value; OnPropertyChanged(); } }
        public string MaxSchool { get => _maxSchool; set { _maxSchool = value; OnPropertyChanged(); } }
        public string MinPharm { get => _minPharm; set { _minPharm = value; OnPropertyChanged(); } }
        public string MaxPharm { get => _maxPharm; set { _maxPharm = value; OnPropertyChanged(); } }

        public string MinClinic { get => _minClinic; set { _minClinic = value; OnPropertyChanged(); } }
        public string MaxClinic { get => _maxClinic; set { _maxClinic = value; OnPropertyChanged(); } }
        public string MinPost { get => _minPost; set { _minPost = value; OnPropertyChanged(); } }
        public string MaxPost { get => _maxPost; set { _maxPost = value; OnPropertyChanged(); } }
        public string MinKinder { get => _minKinder; set { _minKinder = value; OnPropertyChanged(); } }
        public string MaxKinder { get => _maxKinder; set { _maxKinder = value; OnPropertyChanged(); } }
        public string MinRest { get => _minRest; set { _minRest = value; OnPropertyChanged(); } }
        public string MaxRest { get => _maxRest; set { _maxRest = value; OnPropertyChanged(); } }
        public string MinCol { get => _minCol; set { _minCol = value; OnPropertyChanged(); } }
        public string MaxCol { get => _maxCol; set { _maxCol = value; OnPropertyChanged(); } }

        // 4. FILTRY UDOGODNIEŃ
        private int _idxParking = 0, _idxBalcony = 0, _idxElevator = 0, _idxSecurity = 0, _idxStorage = 0;
        public int IdxParking { get => _idxParking; set { _idxParking = value; OnPropertyChanged(); } }
        public int IdxBalcony { get => _idxBalcony; set { _idxBalcony = value; OnPropertyChanged(); } }
        public int IdxElevator { get => _idxElevator; set { _idxElevator = value; OnPropertyChanged(); } }
        public int IdxSecurity { get => _idxSecurity; set { _idxSecurity = value; OnPropertyChanged(); } }
        public int IdxStorage { get => _idxStorage; set { _idxStorage = value; OnPropertyChanged(); } }

        // 5. CHECKBOXY (ŻĄDANIA UŻYTKOWNIKA)
        private bool _reqC = true, _reqPS = true, _reqP = true, _reqA = true, _reqMS = true, _reqDist = true, _reqAm = true;
        public bool ReqCount { get => _reqC; set { _reqC = value; OnPropertyChanged(); } }
        public bool ReqPriceSqm { get => _reqPS; set { _reqPS = value; OnPropertyChanged(); } }
        public bool ReqPrice { get => _reqP; set { _reqP = value; OnPropertyChanged(); } }
        public bool ReqAge { get => _reqA; set { _reqA = value; OnPropertyChanged(); } }
        public bool ReqMarketShare { get => _reqMS; set { _reqMS = value; OnPropertyChanged(); } }
        public bool ReqDistances { get => _reqDist; set { _reqDist = value; OnPropertyChanged(); } }
        public bool ReqAmenities { get => _reqAm; set { _reqAm = value; OnPropertyChanged(); } }

        // 6. WIDOCZNOŚĆ WYNIKÓW (Sterowana tylko przez przycisk Generuj)
        private bool _showC, _showPS, _showP, _showA, _showMS, _showDist, _showAm;
        public bool ShowCount { get => _showC; set { _showC = value; OnPropertyChanged(); } }
        public bool ShowPriceSqm { get => _showPS; set { _showPS = value; OnPropertyChanged(); } }
        public bool ShowPrice { get => _showP; set { _showP = value; OnPropertyChanged(); } }
        public bool ShowAge { get => _showA; set { _showA = value; OnPropertyChanged(); } }
        public bool ShowMarketShare { get => _showMS; set { _showMS = value; OnPropertyChanged(); } }
        public bool ShowDistances { get => _showDist; set { _showDist = value; OnPropertyChanged(); } }
        public bool ShowAmenities { get => _showAm; set { _showAm = value; OnPropertyChanged(); } }

        // 7. WYNIKI (Kpis)
        private AnalysisKpis? _kpis;
        public AnalysisKpis? Kpis { get => _kpis; set { _kpis = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasResults)); } }
        public bool HasResults => Kpis != null;
        public ICommand AnalyzeCommand { get; }

        public DeepAnalysisViewModel()
        {
            AnalyzeCommand = new RelayCommand(async _ => await RunAnalysis());
            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var ranges = await _apiService.GetFilterRangesAsync(AppState.CurrentLogId);
                if (ranges != null)
                {
                    foreach (var c in ranges.Categories.Cities) CitiesGroup.AddItem(c);
                    foreach (var t in ranges.Categories.Types) TypesGroup.AddItem(t);
                    foreach (var o in ranges.Categories.Ownerships) OwnershipsGroup.AddItem(o);
                    foreach (var m in ranges.Categories.Materials) MaterialsGroup.AddItem(m);
                    foreach (var cond in ranges.Categories.Conditions) ConditionsGroup.AddItem(cond);

                    LblPrice = $"Cena PLN ({ranges.Numeric.Price.Min:N0} - {ranges.Numeric.Price.Max:N0}):";
                    LblSqm = $"Metraż m² ({ranges.Numeric.Sqm.Min:N0} - {ranges.Numeric.Sqm.Max:N0}):";
                    LblRooms = $"Liczba Pokoi ({ranges.Numeric.Rooms.Min} - {ranges.Numeric.Rooms.Max}):";
                    LblFloor = $"Piętro ({ranges.Numeric.Floor.Min} - {ranges.Numeric.Floor.Max}):";
                    LblFloorCount = $"Pięter w Bud. ({ranges.Numeric.FloorCount.Min} - {ranges.Numeric.FloorCount.Max}):";
                    LblYear = $"Rok Budowy ({ranges.Numeric.BuildYear.Min} - {ranges.Numeric.BuildYear.Max}):";
                    LblPoi = $"Ilość POI ({ranges.Numeric.PoiCount.Min} - {ranges.Numeric.PoiCount.Max}):";

                    LblDist = $"Do Centrum ({ranges.Numeric.CentreDistance.Min:N1} - {ranges.Numeric.CentreDistance.Max:N1}km):";
                    LblSchool = $"Do Szkoły ({ranges.Numeric.SchoolDistance.Min:N1} - {ranges.Numeric.SchoolDistance.Max:N1}km):";
                    LblPharm = $"Do Apteki ({ranges.Numeric.PharmacyDistance.Min:N1} - {ranges.Numeric.PharmacyDistance.Max:N1}km):";
                    LblClinic = $"Do Przychodni ({ranges.Numeric.ClinicDistance.Min:N1} - {ranges.Numeric.ClinicDistance.Max:N1}km):";
                    LblPost = $"Do Poczty ({ranges.Numeric.PostOfficeDistance.Min:N1} - {ranges.Numeric.PostOfficeDistance.Max:N1}km):";
                    LblKinder = $"Do Przedszkola ({ranges.Numeric.KindergartenDistance.Min:N1} - {ranges.Numeric.KindergartenDistance.Max:N1}km):";
                    LblRest = $"Do Restauracji ({ranges.Numeric.RestaurantDistance.Min:N1} - {ranges.Numeric.RestaurantDistance.Max:N1}km):";
                    LblCol = $"Do Uczelni ({ranges.Numeric.CollegeDistance.Min:N1} - {ranges.Numeric.CollegeDistance.Max:N1}km):";
                }
            }
            catch { }
        }

        private bool? GetBoolValue(int index) => index switch { 1 => true, 2 => false, _ => null };
        private double? ParseOrNull(string val) => double.TryParse(val, out var d) ? d : null;

        private async Task RunAnalysis()
        {
            var req = new DeepAnalysisRequest
            {
                LogId = AppState.CurrentLogId,
                Filters = new AnalysisFilters
                {
                    Cities = CitiesGroup.IsAllSelected ? null : CitiesGroup.GetSelectedNames(),
                    Types = TypesGroup.IsAllSelected ? null : TypesGroup.GetSelectedNames(),
                    Ownerships = OwnershipsGroup.IsAllSelected ? null : OwnershipsGroup.GetSelectedNames(),
                    BuildingMaterials = MaterialsGroup.IsAllSelected ? null : MaterialsGroup.GetSelectedNames(),
                    Conditions = ConditionsGroup.IsAllSelected ? null : ConditionsGroup.GetSelectedNames(),

                    MinPrice = ParseOrNull(MinPrice),
                    MaxPrice = ParseOrNull(MaxPrice),
                    MinSqm = ParseOrNull(MinSqm),
                    MaxSqm = ParseOrNull(MaxSqm),
                    MinRooms = ParseOrNull(MinRooms),
                    MaxRooms = ParseOrNull(MaxRooms),
                    MinFloor = ParseOrNull(MinFloor),
                    MaxFloor = ParseOrNull(MaxFloor),
                    MinFloorCount = ParseOrNull(MinFloorCount),
                    MaxFloorCount = ParseOrNull(MaxFloorCount),
                    MinBuildYear = ParseOrNull(MinYear),
                    MaxBuildYear = ParseOrNull(MaxYear),
                    MinPoiCount = ParseOrNull(MinPoi),
                    MaxPoiCount = ParseOrNull(MaxPoi),

                    MinCentreDistance = ParseOrNull(MinDist),
                    MaxCentreDistance = ParseOrNull(MaxDist),
                    MinSchoolDist = ParseOrNull(MinSchool),
                    MaxSchoolDist = ParseOrNull(MaxSchool),
                    MinPharmacyDist = ParseOrNull(MinPharm),
                    MaxPharmacyDist = ParseOrNull(MaxPharm),
                    MinClinicDist = ParseOrNull(MinClinic),
                    MaxClinicDist = ParseOrNull(MaxClinic),
                    MinPostOfficeDist = ParseOrNull(MinPost),
                    MaxPostOfficeDist = ParseOrNull(MaxPost),
                    MinKindergartenDist = ParseOrNull(MinKinder),
                    MaxKindergartenDist = ParseOrNull(MaxKinder),
                    MinRestaurantDist = ParseOrNull(MinRest),
                    MaxRestaurantDist = ParseOrNull(MaxRest),
                    MinCollegeDist = ParseOrNull(MinCol),
                    MaxCollegeDist = ParseOrNull(MaxCol),

                    HasParkingSpace = GetBoolValue(IdxParking),
                    HasBalcony = GetBoolValue(IdxBalcony),
                    HasElevator = GetBoolValue(IdxElevator),
                    HasSecurity = GetBoolValue(IdxSecurity),
                    HasStorageRoom = GetBoolValue(IdxStorage)
                }
            };

            var response = await _apiService.GetDeepAnalysisAsync(req);

            if (response != null)
            {
                Kpis = response.Kpis;

                // Aktualizujemy widoczność w interfejsie dopiero po przeliczeniu i pobraniu danych z Pythona
                ShowCount = ReqCount;
                ShowPriceSqm = ReqPriceSqm;
                ShowPrice = ReqPrice;
                ShowAge = ReqAge;
                ShowMarketShare = ReqMarketShare;
                ShowDistances = ReqDistances;
                ShowAmenities = ReqAmenities;
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                Kpis = null;

                // Chowamy wszystko
                ShowCount = ShowPriceSqm = ShowPrice = ShowAge = ShowMarketShare = ShowDistances = ShowAmenities = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}