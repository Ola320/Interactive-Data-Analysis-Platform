// src/services/api.js

export const getLogs = async () => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve([
        { id: '1', filename: 'Q1_2024_Sales.csv', date: '2024-04-01T10:00:00Z' },
        { id: '2', filename: 'Q2_2024_Sales.csv', date: '2024-04-15T14:30:00Z' },
      ]);
    }, 500);
  });
};

export const getData = async (id) => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        kpis: {
          totalOffers: 1250,
          avgPrice: 350000,
          medianPrice: 310000,
          avgPricePerSqM: 4500,
        },
        topCities: [
          { name: 'City A', pricePerSqM: 6000 },
          { name: 'City B', pricePerSqM: 5500 },
          { name: 'City C', pricePerSqM: 4800 },
          { name: 'City D', pricePerSqM: 4200 },
        ],
        roomDistribution: [
          { name: '1 Room', value: 15 },
          { name: '2 Rooms', value: 40 },
          { name: '3 Rooms', value: 30 },
          { name: '4+ Rooms', value: 15 },
        ],
        priceVsDistance: [
          { distance: 2, price: 500000 },
          { distance: 5, price: 400000 },
          { distance: 10, price: 300000 },
          { distance: 15, price: 250000 },
          { distance: 20, price: 200000 },
        ]
      });
    }, 500);
  });
};

export const getCityDetails = async (id, city) => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        city,
        totalListings: 150,
        avgPrice: 420000,
        avgPricePerSqM: 5200,
      });
    }, 500);
  });
};
