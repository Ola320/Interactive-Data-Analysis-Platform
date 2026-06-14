// src/services/api.js

export const getLogs = async (filters = {}) => {
  return new Promise((resolve) => {
    setTimeout(() => {
      const logs = [
        { id: '1', filename: 'Q1_2024_Sales.csv', date: '2024-04-01T10:00:00Z' },
        { id: '2', filename: 'Q2_2024_Sales.csv', date: '2024-04-15T14:30:00Z' },
      ];
      if (filters.filename) {
        resolve(logs.filter(l => l.filename.includes(filters.filename)));
      } else {
        resolve(logs);
      }
    }, 500);
  });
};

export const getData = async (id, filters = {}) => {
  return new Promise((resolve) => {
    setTimeout(() => {
      const kpis = {
        totalOffers: 1250,
        avgPrice: 350000,
        medianPrice: 310000,
        avgPricePerSqm: 4500,
      };

      let topCities = [
        { name: 'City A', pricePerSqm: 6000 },
        { name: 'City B', pricePerSqm: 5500 },
        { name: 'City C', pricePerSqm: 4800 },
        { name: 'City D', pricePerSqm: 4200 },
      ];
      if (filters.topCityMinPricePerSqm) {
        topCities = topCities.filter(c => c.pricePerSqm >= filters.topCityMinPricePerSqm);
      }

      const roomDistribution = [
        { name: '1 Room', value: 15 },
        { name: '2 Rooms', value: 40 },
        { name: '3 Rooms', value: 30 },
        { name: '4+ Rooms', value: 15 },
      ];

      const priceVsDistance = [
        { distance: 2, price: 500000 },
        { distance: 5, price: 400000 },
        { distance: 10, price: 300000 },
        { distance: 15, price: 250000 },
        { distance: 20, price: 200000 },
      ];

      resolve({
        kpis,
        topCities,
        roomDistribution,
        priceVsDistance,
      });
    }, 500);
  });
};

export const getCityDetails = async (id, city, filters = {}) => {
  // In real app we'd call backend API with query params. Here we return mock data
  console.log('getCityDetails filters', filters);
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        city,
        totalListings: 150,
        avgPrice: 420000,
        avgPricePerSqm: 5200,
      });
    }, 500);
  });
};
