import React from 'react';
import { Home, Tag, TrendingUp, Grid } from 'lucide-react';

const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0,
  }).format(value);
};

const Card = ({ title, value, icon: Icon, trend }) => (
  <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-5 flex items-start gap-4 hover:shadow-md transition-shadow">
    <div className="p-3 bg-indigo-50 text-indigo-600 rounded-lg">
      <Icon className="w-6 h-6" />
    </div>
    <div>
      <p className="text-sm font-medium text-slate-500 mb-1">{title}</p>
      <h3 className="text-2xl font-bold text-slate-900">{value}</h3>
      {trend && (
        <p className="text-xs font-medium mt-1 text-emerald-600">
          +{trend}% from last month
        </p>
      )}
    </div>
  </div>
);

const SummaryCards = ({ data }) => {
  if (!data) return null;

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <Card
        title="Total Offers"
        value={data.totalOffers.toLocaleString()}
        icon={Home}
        trend={12}
      />
      <Card
        title="Avg Price"
        value={formatCurrency(data.avgPrice)}
        icon={Tag}
      />
      <Card
        title="Median Price"
        value={formatCurrency(data.medianPrice)}
        icon={TrendingUp}
      />
      <Card
        title="Avg Price / m²"
        value={formatCurrency(data.avgPricePerSqM)}
        icon={Grid}
        trend={5}
      />
    </div>
  );
};

export default SummaryCards;
