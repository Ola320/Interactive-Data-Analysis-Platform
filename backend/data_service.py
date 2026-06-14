import pandas
import json
import numpy as np
import datetime
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

def get_city_analytics(df: DataFrame, city: str):
    df_city = df[df['city'].str.lower() == city.lower()].copy()

    avg_m = round(df_city['price'].mean(),0)
    avg_price_m2 = (df_city['price']/df_city['squareMeters']).mean()

    distance_km = [
        {'dist':round(float(r['centreDistance']),1),'price': int(r['price'])}
        for _, r in df_city.iterrows()
    ]
    return {
        'city': city,
        'total_listings': int(len(df_city)),
        'avg_price': float(avg_m),
        'avg_price_per_sqm': float(avg_price_m2)
    }

def perform_deep_analysis(df: pd.DataFrame, filters: dict, requested_charts: list):
    original_len = len(df)
    
    # --- 1. FILTROWANIE DANYCH KATEGORIALNYCH ---
    categorical_mappings = {
        'city': 'cities',
        'type': 'types',
        'ownership': 'ownerships',
        'buildingMaterial': 'building_materials',
        'condition': 'conditions'
    }
    
    for col, filter_key in categorical_mappings.items():
        if col in df.columns and filters.get(filter_key):
            df = df[df[col].isin(filters[filter_key])]

    # --- 2. FILTROWANIE DANYCH NUMERYCZNYCH (Wszystkie dystanse) ---
    numeric_mappings = [
        ('price', 'min_price', 'max_price'),
        ('squareMeters', 'min_sqm', 'max_sqm'),
        ('rooms', 'min_rooms', 'max_rooms'),
        ('floor', 'min_floor', 'max_floor'),
        ('floorCount', 'min_floor_count', 'max_floor_count'),
        ('buildYear', 'min_build_year', 'max_build_year'),
        ('centreDistance', 'min_centre_distance', 'max_centre_distance'),
        ('poiCount', 'min_poi', 'max_poi'),
        ('schoolDistance', 'min_school_dist', 'max_school_dist'),
        ('clinicDistance', 'min_clinic_dist', 'max_clinic_dist'),
        ('pharmacyDistance', 'min_pharmacy_dist', 'max_pharmacy_dist'),
        ('postOfficeDistance', 'min_post_office_dist', 'max_post_office_dist'),
        ('kindergartenDistance', 'min_kindergarten_dist', 'max_kindergarten_dist'),
        ('restaurantDistance', 'min_restaurant_dist', 'max_restaurant_dist'),
        ('collegeDistance', 'min_college_dist', 'max_college_dist')
    ]
    
    for col, min_key, max_key in numeric_mappings:
        if col in df.columns:
            if filters.get(min_key) is not None:
                df = df[df[col] >= filters[min_key]]
            if filters.get(max_key) is not None:
                df = df[df[col] <= filters[max_key]]

    # --- 3. FILTROWANIE DANYCH BOOLEAN ---
    boolean_mappings = {
        'hasParkingSpace': 'has_parking',
        'hasBalcony': 'has_balcony',
        'hasElevator': 'has_elevator',
        'hasSecurity': 'has_security',
        'hasStorageRoom': 'has_storage_room'
    }
    
    for col, filter_key in boolean_mappings.items():
        req_val = filters.get(filter_key)
        if col in df.columns and req_val is not None:
            target_str = 'yes' if req_val else 'no'
            df = df[df[col].astype(str).str.lower() == target_str]

    if df.empty:
        return {"error": "No data matches the given criteria."}

    df['price_sqm'] = df['price'] / df['squareMeters']

    # --- 4. WYLICZANIE PODSTAWOWYCH KPI ---
    kpis = {
        "count": len(df),
        "market_share": round((len(df) / original_len) * 100, 2),
        "avg_price": round(df['price'].mean(), 0),
        "avg_price_sqm": round(df['price_sqm'].mean(), 0)
    }

    if 'buildYear' in df.columns and not df['buildYear'].dropna().empty:
        kpis['avg_building_age'] = round(datetime.datetime.now().year - df['buildYear'].dropna().mean(), 1)

    # WYLICZANIE ŚREDNICH DYSTANSÓW
    dist_cols = {
        'centreDistance': 'avg_centre_distance',
        'schoolDistance': 'avg_school_dist',
        'clinicDistance': 'avg_clinic_dist',
        'postOfficeDistance': 'avg_post_office_dist',
        'kindergartenDistance': 'avg_kindergarten_dist',
        'restaurantDistance': 'avg_restaurant_dist',
        'collegeDistance': 'avg_college_dist',
        'pharmacyDistance': 'avg_pharmacy_dist'
    }
    for col, key in dist_cols.items():
        if col in df.columns and not df[col].dropna().empty:
            kpis[key] = round(df[col].mean(), 2)

    # --- 5. WYLICZANIE WARTOŚCI UDOGODNIEŃ (Różnica w średniej cenie) ---
    for col, key in [('hasParkingSpace', 'cost_parking'), ('hasBalcony', 'cost_balcony'),
                     ('hasElevator', 'cost_elevator'), ('hasSecurity', 'cost_security'),
                     ('hasStorageRoom', 'cost_storage')]:
        if col in df.columns:
            yes_mean = df[df[col].astype(str).str.lower() == 'yes']['price'].mean()
            no_mean = df[df[col].astype(str).str.lower() == 'no']['price'].mean()
            if pd.notna(yes_mean) and pd.notna(no_mean):
                kpis[key] = round(yes_mean - no_mean, 0)

    # Zwracamy wszystkie analizy (C# ukryje te, których użytkownik nie chce)
    return {
        "kpis": kpis
    }


