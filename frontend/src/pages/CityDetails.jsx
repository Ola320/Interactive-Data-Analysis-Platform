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
  const [cityData, setCityData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) return;

    setLoading(true);
    setError(null);
    try {
      const data = await getCityDetails('latest', searchTerm);
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
        <form onSubmit={handleSearch} className="flex gap-4 max-w-2xl">
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
          <button
            type="submit"
            disabled={loading}
            className="px-6 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50"
          >
            {loading ? 'Searching...' : 'Search'}
          </button>
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
