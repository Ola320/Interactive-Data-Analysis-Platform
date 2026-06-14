import React, { useState } from 'react';
import { getCityDetails } from '../services/api';
import { Search, MapPin, TrendingUp, Home } from 'lucide-react';

const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0,
  }).format(value);
};

const CityDetails = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [minRooms, setMinRooms] = useState('');
  const [maxRooms, setMaxRooms] = useState('');
  const [minSqm, setMinSqm] = useState('');
  const [maxSqm, setMaxSqm] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [cityData, setCityData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) return;

    setLoading(true);
    setError(null);
    try {
      const filters = {
        min_rooms: minRooms ? Number(minRooms) : undefined,
        max_rooms: maxRooms ? Number(maxRooms) : undefined,
        min_sqm: minSqm ? Number(minSqm) : undefined,
        max_sqm: maxSqm ? Number(maxSqm) : undefined,
        min_price: minPrice ? Number(minPrice) : undefined,
        max_price: maxPrice ? Number(maxPrice) : undefined,
      };

      const data = await getCityDetails('latest', searchTerm, filters);
      setCityData(data);
    } catch (err) {
      setError("Failed to fetch data for this city.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-7xl mx-auto">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">City Lookup</h1>
        <p className="text-sm text-slate-500 mt-1">Search for specific city metrics and real estate data.</p>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6 mb-8">
        <form onSubmit={handleSearch} className="flex flex-col gap-4 max-w-2xl">
          <div className="relative flex-1">
            <Search className="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Enter city name (e.g. 'City A')..."
              className="w-full pl-12 pr-4 py-3 rounded-lg bg-slate-50 border border-slate-300 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            />
          </div>
          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="block text-xs text-slate-500 mb-1">Rooms (column: rooms)</label>
              <div className="flex gap-2">
                <select
                  value={minRooms}
                  onChange={(e) => setMinRooms(e.target.value)}
                  className="w-24 pl-2 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                >
                  <option value="">Any</option>
                  <option value="0">0</option>
                  <option value="1">1</option>
                  <option value="2">2</option>
                  <option value="3">3</option>
                  <option value="4">4</option>
                  <option value="5">5</option>
                  <option value="6">6</option>
                  <option value="7">7+</option>
                </select>
                <select
                  value={maxRooms}
                  onChange={(e) => setMaxRooms(e.target.value)}
                  className="w-24 pl-2 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                >
                  <option value="">Any</option>
                  <option value="0">0</option>
                  <option value="1">1</option>
                  <option value="2">2</option>
                  <option value="3">3</option>
                  <option value="4">4</option>
                  <option value="5">5</option>
                  <option value="6">6</option>
                  <option value="7">7+</option>
                </select>
              </div>
            </div>

            <div>
              <label className="block text-xs text-slate-500 mb-1">Area m² (column: squareMeters)</label>
              <div className="flex gap-2">
                <input
                  type="number"
                  value={minSqm}
                  onChange={(e) => setMinSqm(e.target.value)}
                  placeholder="Min"
                  className="w-32 pl-3 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                />
                <input
                  type="number"
                  value={maxSqm}
                  onChange={(e) => setMaxSqm(e.target.value)}
                  placeholder="Max"
                  className="w-32 pl-3 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                />
              </div>
            </div>

            <div>
              <label className="block text-xs text-slate-500 mb-1">Price (column: price)</label>
              <div className="flex gap-2">
                <input
                  type="number"
                  value={minPrice}
                  onChange={(e) => setMinPrice(e.target.value)}
                  placeholder="Min"
                  className="w-40 pl-3 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                />
                <input
                  type="number"
                  value={maxPrice}
                  onChange={(e) => setMaxPrice(e.target.value)}
                  placeholder="Max"
                  className="w-40 pl-3 py-2 rounded-lg bg-slate-50 border border-slate-300 text-sm"
                />
              </div>
            </div>
          </div>

          <div className="mt-2">
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50"
            >
              {loading ? 'Searching...' : 'Search'}
            </button>
          </div>
        </form>
      </div>

      {error && (
        <div className="p-4 bg-red-50 text-red-600 rounded-lg mb-8">
          {error}
        </div>
      )}

      {cityData && !loading && (
        <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
          <div className="flex items-center gap-3 mb-6">
            <div className="p-3 bg-indigo-100 rounded-full text-indigo-600">
              <MapPin className="w-6 h-6" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-slate-900">{cityData.city}</h2>
              <p className="text-sm text-slate-500">Market Overview</p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
              <div className="flex items-center justify-between mb-4">
                <p className="text-sm font-medium text-slate-500">Total Listings</p>
                <Home className="w-5 h-5 text-indigo-400" />
              </div>
              <h3 className="text-3xl font-bold text-slate-900">{cityData.totalListings}</h3>
            </div>
            
            <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
              <div className="flex items-center justify-between mb-4">
                <p className="text-sm font-medium text-slate-500">Average Price</p>
                <TrendingUp className="w-5 h-5 text-emerald-500" />
              </div>
              <h3 className="text-3xl font-bold text-slate-900">{formatCurrency(cityData.avgPrice)}</h3>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
              <div className="flex items-center justify-between mb-4">
                <p className="text-sm font-medium text-slate-500">Avg Price / m²</p>
                <MapPin className="w-5 h-5 text-amber-500" />
              </div>
              <h3 className="text-3xl font-bold text-slate-900">{formatCurrency(cityData.avgPricePerSqM)}</h3>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CityDetails;
