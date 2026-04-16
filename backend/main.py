from fastapi import FastAPI, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
import uvicorn
import sqlite3
import json
import os
import pandas as pd
from datetime import datetime
from data_service import process_apartment_data

app = FastAPI(title="Analiza Nieruchomości API")

# Zabezpieczenie CORS - pozwala frontendowi (HTML) połączyć się z backendem
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Tworzenie folderu na pliki, jeśli nie istnieje
if not os.path.exists("uploads"):
    os.makedirs("uploads")

# Funkcja pomocnicza do łączenia z bazą danych
def get_db_connection():
    conn = sqlite3.connect("logs.db")
    conn.row_factory = sqlite3.Row
    return conn

# Inicjalizacja bazy danych SQLite
def init_db():
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS logs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT,
            date TEXT,
            path TEXT,
            stats JSON
        )
    ''')
    conn.commit()
    conn.close()

# Uruchamiamy tworzenie bazy na starcie
init_db()

# Endpoint 1: Wgrywanie pliku
@app.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    raw_data = await file.read()
    
    # Przetwarzanie danych przez naszą usługę w Pythonie
    wyniki = process_apartment_data(raw_data)
    
    # Zapis pliku fizycznie na dysk serwera
    filepath = f"uploads/{file.filename}"
    with open(filepath, "wb") as f:
        f.write(raw_data)
        
    # Zapis metadanych i wyliczonych statystyk do bazy danych
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute(
        "INSERT INTO logs (name, date, path, stats) VALUES (?, ?, ?, ?)", 
        (file.filename, datetime.now().isoformat(), filepath, json.dumps(wyniki))
    )
    conn.commit()
    conn.close()
    
    return wyniki

# Endpoint 2: Pobieranie listy historii wgranych plików
@app.get("/logs")
async def get_logs_history():
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT id, name, date FROM logs ORDER BY id DESC")
    rows = cursor.fetchall()
    conn.close()
    
    logs_list = [dict(row) for row in rows]
    return logs_list

# Endpoint 3: Pobieranie przeliczonych danych globalnych z wybranego logu z bazy
@app.get("/data/{log_id}")
async def get_log_data(log_id: int):
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT stats FROM logs WHERE id = ?", (log_id,))
    row = cursor.fetchone()
    conn.close()
    
    if not row:
        return {"error": "Brak danych dla tego logu."}
        
    return json.loads(row["stats"])

# Endpoint 4: Lupa na miasto - liczenie wykresów na żywo dla wybranego miasta
@app.get("/city_details/{log_id}/{city_name}")
async def get_city_details(log_id: int, city_name: str):
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT path FROM logs WHERE id = ?", (log_id,))
    row = cursor.fetchone()
    conn.close()
    
    if not row:
        return {"error": "Plik nie istnieje w bazie."}

    filepath = row["path"]
    
    # Wczytujemy z dysku pełny plik CSV i filtrujemy go po nazwie miasta
    df = pd.read_csv(filepath)
    df_city = df[df['city'].str.lower() == city_name.lower()]
    
    # Usuwamy puste wiersze tylko dla kolumn, których używamy do tych dwóch konkretnych wykresów
    df_city = df_city.dropna(subset=['price', 'squareMeters', 'rooms', 'centreDistance'])
    
    if df_city.empty:
        return {"error": f"Brak poprawnych danych dla miasta {city_name}."}

    # Obliczanie KPI (Kluczowych wskaźników)
    kpi_n = len(df_city)
    kpi_avg = round(float(df_city['price'].mean()), 0)
    kpi_m2 = round(float((df_city['price'] / df_city['squareMeters']).mean()), 0)

    # Wykres 1 dla miasta: Średnia cena w zależności od liczby pokoi
    pokoje_stat = df_city.groupby('rooms')['price'].mean().reset_index()
    wykres_pokoje = []
    for _, row in pokoje_stat.iterrows():
        wykres_pokoje.append({"x": f"{int(row['rooms'])} pok.", "y": float(row['price'])})
    
    # Wykres 2 dla miasta: Punktowy odległość od centrum vs cena (BIERZEMY WSZYSTKIE WIERSZE - brak próbowania)
    wykres_centrum = []
    for _, row in df_city.iterrows():
        wykres_centrum.append({"x": float(row['centreDistance']), "y": float(row['price'])})

    return {
        "kpi": {
            "n": kpi_n, 
            "avg": kpi_avg, 
            "m2": kpi_m2
        },
        "charts": {
            "pokoje": wykres_pokoje,
            "centrum": wykres_centrum
        }
    }

# Uruchomienie serwera uvicorn (lokalnie na porcie 8000)
if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)