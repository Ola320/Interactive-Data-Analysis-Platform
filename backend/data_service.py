import pandas
import json

import pandas as pd
from pandas import DataFrame
from unicodedata import numeric


def clean_data(df: pd.DataFrame) -> pd.DataFrame:

    df = df.dropna(subset=['price','squareMeters'])

    df['price'] = pd.to_numeric(df['price'],errors='coerce')
    df['squareMeters'] = pd.to_numeric(df['squareMeters'],errors='coerce')

    df = df.dropna(subset=['price','squareMeters'])

    df = df.drop_duplicates()
    df = df[df['floor'] <= df['floorCount']]

    numerical_cols = df.select_dtypes(include=['number']).columns
    df[numerical_cols] = df[numerical_cols].fillna(df[numerical_cols].median())

    object_cols = df.select_dtypes(include='object').columns
    for col in object_cols:
        if not df[col].mode().empty:
            df[col] = df[col].fillna(df[col].mode()[0])

    df = df.drop_duplicates(subset=['latitude','longitude','price','squareMeters'])
    df = df[df['floor']<= df['floorCount']]

    return df

def process_apartament_data(df: pd.DataFrame):
    df['price_per_m2'] = df['price']/df['squareMeters']

    df['price_per_m2'] = df['price']/df['squareMeters']

    global_stats = {
        "total_offers": int(len(df)),
        "avg_price": round(float(df['price'].mean()),0),
        "median_price": round(float(df['price'].median()),0),
        "average_price_per_m^2": round(float(df['price_per_m2'].median()),0),
        "average_price_per_m^2_median":round(float(df['price_per_m2'].median()),0)
    }

    city_ranking = df.groupby('city')['price_per_m2'].mean().sort_values(ascending=False).head()
    city_chart = [{'city':city, 'value':round(val,0)} for city,val in city_ranking.items()]

    rooms_dist = df['rooms'].value_counts().sort_index()
    rooms_chart = [{'name': str(room), "value": int(val)} for room, val in rooms_dist.items()]

    scatter_data = df[['squareMeters','price']].sample(n=min(500,len(df))).to_dict(orient='records')

    trend_data = df.groupby('buildYear')['price_per_m2'].mean().sort_index().reset_index()
    trend_chart = [{"year": int(row['buildYear']), "avg_price": round(row['price_per_m2'], 0)}
                   for _, row in trend_data.iterrows() if row['buildYear'] > 1800]


    df['distance_km'] = df['centreDistance'].round(0)
    dist_ranking = df.groupby('distance_km')['price_per_m2'].mean()
    dist_chart = [{'distance':distance, 'value':value} for distance, value in dist_ranking.items()]

    return {
        'summary':global_stats,
        "charts":{
            'city_chart':city_chart,
            'rooms_chart':rooms_chart,
            'price_vs_distance':dist_chart,
            'trends':trend_chart
        },
        'scratter_points':scatter_data

    }

def get_city_analytics(df: DataFrame, city: str,
                       min_rooms: int = None, max_rooms: int = None,
                       min_sqm: float = None, max_sqm: float = None,
                       min_price: float = None, max_price: float = None):
    # Filter by city first
    df_city = df[df['city'].str.lower() == city.lower()].copy()

    # Apply optional filters if columns exist
    if df_city.empty:
        return None

    if min_rooms is not None and 'rooms' in df_city.columns:
        df_city = df_city[df_city['rooms'] >= min_rooms]
    if max_rooms is not None and 'rooms' in df_city.columns:
        df_city = df_city[df_city['rooms'] <= max_rooms]

    if min_sqm is not None and 'squareMeters' in df_city.columns:
        df_city = df_city[df_city['squareMeters'] >= min_sqm]
    if max_sqm is not None and 'squareMeters' in df_city.columns:
        df_city = df_city[df_city['squareMeters'] <= max_sqm]

    if min_price is not None and 'price' in df_city.columns:
        df_city = df_city[df_city['price'] >= min_price]
    if max_price is not None and 'price' in df_city.columns:
        df_city = df_city[df_city['price'] <= max_price]

    if df_city.empty:
        return None

    avg_m = round(df_city['price'].mean(), 0)
    avg_price_m2 = (df_city['price'] / df_city['squareMeters']).mean()

    return {
        'city': city,
        'totalListings': int(len(df_city)),
        'avgPrice': float(avg_m),
        'avgPricePerSqM': float(round(avg_price_m2, 0))
    }


