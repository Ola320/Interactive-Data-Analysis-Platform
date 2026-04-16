import pandas as pd
import numpy as np
import io

def process_apartment_data(contents: bytes) -> dict:
    # 1. Wczytanie danych z wgranego pliku CSV do obiektu DataFrame
    df = pd.read_csv(io.BytesIO(contents))
    
    # Wymuszenie typów numerycznych na kluczowych kolumnach (zabezpieczenie)
    df['price'] = pd.to_numeric(df['price'], errors='coerce')
    df['squareMeters'] = pd.to_numeric(df['squareMeters'], errors='coerce')
    
    # 2. Czyszczenie danych (usuwamy wiersze, które nie maj¹ ceny, metra¿u lub miasta)
    df_cleaned = df.dropna(subset=['price', 'squareMeters', 'city']).copy()

    # 3. Wyliczanie podstawowych statystyk globalnych
    if not df_cleaned.empty:
        srednia_cena = round(float(df_cleaned['price'].mean()), 0)
    else:
        srednia_cena = 0
    
    # Pobieramy listê wszystkich unikalnych miast i sortujemy j¹ alfabetycznie dla wyszukiwarki
    lista_miast = sorted(df_cleaned['city'].unique().astype(str).tolist())

    # 4. Przygotowanie danych pod wykresy globalne

    # A. Wykres: Rozk³ad Miast (Pobieramy wszystkie 15 miast z bazy)
    top_cities = df_cleaned['city'].value_counts().head(15)
    data_miasta = []
    for nazwa, ilosc in top_cities.items():
        data_miasta.append({"x": str(nazwa), "y": int(ilosc)})

    # B. Wykres: Histogram Cen (Dzielimy na 10 równych przedzia³ów)
    counts, bins = np.histogram(df_cleaned['price'], bins=10)
    data_hist = []
    for i in range(len(counts)):
        przedzial = f"{int(bins[i]/1000)}k - {int(bins[i+1]/1000)}k"
        data_hist.append({"x": przedzial, "y": int(counts[i])})

    # C. Wykres punktowy: Metra¿ vs Cena (Z próbkowaniem do 1000 punktów dla ca³ej Polski)
    df_scatter = df_cleaned[['squareMeters', 'price']]
    if len(df_scatter) > 1000:
        df_scatter = df_scatter.sample(n=1000, random_state=42)
        
    data_scatter = []
    for _, row in df_scatter.iterrows():
        data_scatter.append({"x": float(row['squareMeters']), "y": float(row['price'])})

    # 5. Spakowanie ostatecznego wyniku do s³ownika
    return {
        "info": {
            "wiersze": len(df_cleaned),
            "srednia": srednia_cena,
            "miasta": lista_miast
        },
        "wykresy_globalne": {
            "miasta": data_miasta,
            "histogram": data_hist,
            "scatter": data_scatter
        }
    }