from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, EmailStr
import uvicorn
import os
import io
import json
import pandas as pd
from datetime import datetime, timedelta
from contextlib import asynccontextmanager
import bcrypt
import jwt
from typing import List, Optional
from fastapi.responses import JSONResponse

from data_service import get_city_analytics, clean_data, process_apartament_data
from database import get_db_connection, init_db
from data_service import get_city_analytics, clean_data, process_apartament_data, perform_deep_analysis

# JWT CONFIG
JWT_SECRET = "super_secret_key"
JWT_ALGORITHM = "HS256"

# MODELE PYDANTIC (Zintegrowane z polem e-mail)
class RegisterModel(BaseModel):
    username: str
    email: str  # Dodane pole e-mail do rejestracji
    password: str

class LoginModel(BaseModel):
    username: str
    password: str

def create_jwt_token(username: str):
    payload = {
        "sub": username,
        "exp": datetime.utcnow() + timedelta(hours=12)
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

class AnalysisFilters(BaseModel):
    cities: Optional[List[str]] = []
    types: Optional[List[str]] = []
    ownerships: Optional[List[str]] = []
    building_materials: Optional[List[str]] = []
    conditions: Optional[List[str]] = []

    min_price: Optional[float] = None
    max_price: Optional[float] = None
    min_sqm: Optional[float] = None
    max_sqm: Optional[float] = None
    min_rooms: Optional[float] = None
    max_rooms: Optional[float] = None
    min_floor: Optional[float] = None
    max_floor: Optional[float] = None
    min_floor_count: Optional[float] = None
    max_floor_count: Optional[float] = None
    min_build_year: Optional[float] = None
    max_build_year: Optional[float] = None

    min_centre_distance: Optional[float] = None
    max_centre_distance: Optional[float] = None
    min_poi: Optional[float] = None
    max_poi: Optional[float] = None
    
    # WSZYSTKIE DYSTANSE
    min_school_dist: Optional[float] = None
    max_school_dist: Optional[float] = None
    min_clinic_dist: Optional[float] = None
    max_clinic_dist: Optional[float] = None
    min_post_office_dist: Optional[float] = None
    max_post_office_dist: Optional[float] = None
    min_kindergarten_dist: Optional[float] = None
    max_kindergarten_dist: Optional[float] = None
    min_restaurant_dist: Optional[float] = None
    max_restaurant_dist: Optional[float] = None
    min_college_dist: Optional[float] = None
    max_college_dist: Optional[float] = None
    min_pharmacy_dist: Optional[float] = None
    max_pharmacy_dist: Optional[float] = None

    has_parking: Optional[bool] = None
    has_balcony: Optional[bool] = None
    has_elevator: Optional[bool] = None
    has_security: Optional[bool] = None
    has_storage_room: Optional[bool] = None

class DeepAnalysisRequest(BaseModel):
    log_id: int
    filters: AnalysisFilters
    requested_kpis: Optional[List[str]] = []
    requested_charts: List[int]


# LIFESPAN APP (Zarządzanie startem i końcem aplikacji)
@asynccontextmanager
async def lifespan(app: FastAPI):
    if not os.path.exists("uploads"):
        os.makedirs("uploads")
    init_db()  # Inicjalizacja bazy (teraz tworzy tabelę users z polem email)
    yield

app = FastAPI(title="Analiza Nieruchomości API", lifespan=lifespan)

# KONFIGURACJA CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# --- AUTH ENDPOINTS ---

@app.post("/register")
async def register(user: RegisterModel):
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Walidacja unikalności loginu oraz e-maila po stronie backendu
    cursor.execute("SELECT id FROM users WHERE username = ? OR email = ?", (user.username, user.email))
    if cursor.fetchone():
        conn.close()
        raise HTTPException(status_code=400, detail="Nazwa użytkownika lub adres e-mail jest już zajęty.")
        
    password_hash = bcrypt.hashpw(user.password.encode(), bcrypt.gensalt()).decode()
    try:
        cursor.execute(
            "INSERT INTO users (username, email, password_hash) VALUES (?, ?, ?)", 
            (user.username, user.email, password_hash)
        )
        conn.commit()
    except Exception as e:
        conn.close()
        raise HTTPException(status_code=500, detail="Błąd zapisu w bazie danych: " + str(e))
        
    conn.close()
    return {"msg": "Rejestracja udana"}

@app.post("/login")
async def login(user: LoginModel):
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute("SELECT password_hash FROM users WHERE username = ?", (user.username,))
    row = cursor.fetchone()
    conn.close()
    
    if not row:
        raise HTTPException(status_code=401, detail="Nieprawidłowy login lub hasło")
        
    password_hash = row["password_hash"]
    if not bcrypt.checkpw(user.password.encode(), password_hash.encode()):
        raise HTTPException(status_code=401, detail="Nieprawidłowy login lub hasło")
        
    token = create_jwt_token(user.username)
    return {"access_token": token, "token_type": "bearer"}

# --- POZOSTAŁE ENDPOINTY (DANE I LOGI) ---

@app.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    if not file.filename.endswith('.csv'):
        raise HTTPException(status_code=400, detail="Tylko pliki CSV są akceptowane.")

    try:
        content = await file.read()
        df_raw = pd.read_csv(io.BytesIO(content))

        print(f"debubL naglownki w plikue {df_raw.columns.tolist()}")

        print(f"pierwsze wiersze {df_raw.head()}")



        # 2. Czyszczenie i analiza (Twój data_service)
        df_clean = clean_data(df_raw)
        
        if df_clean.empty:
            raise HTTPException(status_code=400, detail="Po wyczyszczeniu danych plik jest pusty.")

        wyniki = process_apartament_data(df_clean)
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        filepath = f"uploads/cleaned_{timestamp}_{file.filename}"
        df_clean.to_csv(filepath, index=False)

        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            "INSERT INTO logs (name, date, path, stats) VALUES (?, ?, ?, ?)",
            (file.filename, datetime.now().isoformat(), filepath, json.dumps(wyniki))
        )
        conn.commit()
        log_id = cursor.lastrowid
        conn.close()

        return {
            "id": log_id,
            "stats": wyniki
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Błąd przetwarzania: {str(e)}")

@app.get("/logs")
async def get_logs():
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT id, name, date FROM logs ORDER BY id DESC")
    rows = cursor.fetchall()
    conn.close()
    return [dict(row) for row in rows]

@app.get("/logs/{log_id}")
async def get_log_details(log_id: int):
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT stats FROM logs WHERE id = ?", (log_id,))
    row = cursor.fetchone()
    conn.close()

    if not row:
        raise HTTPException(status_code=404, detail="Nie znaleziono logu o tym ID.")

    return json.loads(row["stats"])

@app.get("/city_details/{log_id}/{city_name}")
async def city_details(log_id: int, city_name: str):
    conn = get_db_connection()
    row = conn.execute("SELECT path FROM logs WHERE id = ?",(log_id,)).fetchone()
    conn.close()

    if not row:
        raise HTTPException(status_code=404, detail='Nie znaleziono takiego logu raportu.')

    df = pd.read_csv(row['path'])
    analiza = get_city_analytics(df, city_name)

    if not analiza:
        raise HTTPException(status_code=404, detail='Nie znaleziono danych dla podanego miasta.')

    return analiza

@app.delete("/logs/{log_id}")
async def delete_log(log_id: int):
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("SELECT path FROM logs WHERE id = ?", (log_id,))
    row = cursor.fetchone()

    if row:
        if os.path.exists(row["path"]):
            os.remove(row["path"])

        cursor.execute("DELETE FROM logs WHERE id = ?", (log_id,))
        conn.commit()
        conn.close()
        return {"message": "Usunięto pomyślnie"}

    conn.close()
    raise HTTPException(status_code=404, detail="Log nie istnieje")

@app.put("/logs/{log_id}")
async def rename_log(log_id: int, name: str):
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute("UPDATE logs SET name = ? WHERE id = ?", (name, log_id))
    if cursor.rowcount == 0:
        conn.close()
        raise HTTPException(status_code=404, detail="Log nie istnieje")
    conn.commit()
    conn.close()
    return {"message": "Zmieniono nazwę"}


import sqlite3
import json


@app.get("/predict")
async def predict_price(city: str, rooms: int, distance: float, sqm: float):
    conn = get_db_connection()
    cursor = conn.cursor()

    # Pobieramy ostatnie statystyki z bazy
    cursor.execute("SELECT stats FROM logs ORDER BY id DESC LIMIT 1")
    row = cursor.fetchone()
    conn.close()

    base_price_per_m2 = 10000  # Domyślna cena, jeśli baza jest pusta

    if row:
        all_stats = json.loads(row[0])
        # Szukamy ceny w city_chart (którą już generujesz w process_apartament_data)
        city_data = next((item for item in all_stats['charts']['city_chart']
                          if item['city'].lower() == city.lower()), None)

        if city_data:
            base_price_per_m2 = city_data['value']

    # Logika wyliczania ceny
    # (sqm * cena_miasta) + bonus za pokoje - kara za dystans (każdy km dalej = taniej)
    predicted = (sqm * base_price_per_m2) + (rooms * 5000) - (distance * 2000)

    return {
        "city": city.capitalize(),
        "predicted_price": max(0, round(predicted, 2)),
        "source": "database" if city_data else "default"
    }

@app.post("/deep_analysis")
async def create_deep_analysis(req: DeepAnalysisRequest):
    try:
        conn = get_db_connection()
        row = conn.execute("SELECT path FROM logs WHERE id = ?", (req.log_id,)).fetchone()
        conn.close() # Od razu zamykamy połączenie z bazą
        
        if not row:
            raise HTTPException(status_code=404, detail="Nie znaleziono pliku źródłowego.")

        filepath = row['path']
        df = pd.read_csv(filepath)

        # Filtrujemy dane i tworzymy analizę (przekazujemy puste tablice na wykresy, bo ich już nie ma)
        wyniki = perform_deep_analysis(df, req.filters.dict(), [])
        
        if "error" in wyniki:
            return JSONResponse(status_code=400, content={"detail": wyniki["error"]})

        # ZWRACAMY WYNIK W LOCIE (Brak zapisywania do bazy danych!)
        return wyniki

    except Exception as e:
        return JSONResponse(status_code=500, content={"detail": "Błąd generowania analizy: " + str(e)})
# ==============================================================
# Endpoint pomocniczy, żeby WPF wiedział, z jakich miast można wybierać w danym pliku
@app.get("/cities/{log_id}")
async def get_available_cities(log_id: int):
    try:
        conn = get_db_connection()
        row = conn.execute("SELECT path FROM logs WHERE id = ?", (log_id,)).fetchone()
        conn.close()
        
        if not row:
            raise HTTPException(status_code=404, detail="Brak pliku")
            
        df = pd.read_csv(row['path'], usecols=['city'])
        cities = df['city'].dropna().unique().tolist()
        return sorted(cities)
    except Exception as e:
        return JSONResponse(status_code=500, content={"detail": str(e)})

@app.get("/filter_ranges/{log_id}")
async def get_filter_ranges(log_id: int):
    try:
        conn = get_db_connection()
        row = conn.execute("SELECT path FROM logs WHERE id = ?", (log_id,)).fetchone()
        conn.close()

        if not row or not os.path.exists(row['path']):
            raise HTTPException(status_code=404, detail="Brak pliku")

        df = pd.read_csv(row['path'])

        def get_unique(col):
            if col in df.columns:
                return sorted([str(x) for x in df[col].dropna().unique()])
            return []

        def get_min_max(col):
            if col in df.columns and not df[col].dropna().empty:
                return {"min": float(df[col].min()), "max": float(df[col].max())}
            return {"min": 0.0, "max": 0.0}

        return {
            "categories": {
                "cities": get_unique('city'),
                "types": get_unique('type'),
                "materials": get_unique('buildingMaterial'),
                "conditions": get_unique('condition'),
                "ownerships": get_unique('ownership')
            },
            "numeric": {
                "price": get_min_max('price'),
                "sqm": get_min_max('squareMeters'),
                "rooms": get_min_max('rooms'),
                "floor": get_min_max('floor'),
                "floorCount": get_min_max('floorCount'),
                "buildYear": get_min_max('buildYear'),
                "centreDistance": get_min_max('centreDistance'),
                "poiCount": get_min_max('poiCount'),
                "schoolDistance": get_min_max('schoolDistance'),
                "pharmacyDistance": get_min_max('pharmacyDistance'),
                "clinicDistance": get_min_max('clinicDistance'),
                "postOfficeDistance": get_min_max('postOfficeDistance'),
                "kindergartenDistance": get_min_max('kindergartenDistance'),
                "restaurantDistance": get_min_max('restaurantDistance'),
                "collegeDistance": get_min_max('collegeDistance')
            }
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)