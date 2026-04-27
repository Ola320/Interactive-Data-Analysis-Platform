from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import uvicorn
import os
import io
import json
import pandas as pd
from datetime import datetime
from contextlib import asynccontextmanager

from backend.data_service import get_city_analytics
# Import Twoich modułów
from database import get_db_connection, init_db
from data_service import clean_data, process_apartament_data



@asynccontextmanager
async def lifespan(app: FastAPI):
    if not os.path.exists("uploads"):
        os.makedirs("uploads")
    init_db()
    yield



app = FastAPI(title="Analiza Nieruchomości API",lifespan=lifespan)

# Konfiguracja CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    if not file.filename.endswith('.csv'):
        raise HTTPException(status_code=400, detail="Tylko pliki CSV są akceptowane.")

    try:
        # 1. Odczyt surowych danych
        content = await file.read()
        df_raw = pd.read_csv(io.BytesIO(content))

        # 2. Czyszczenie i analiza (Twój data_service)
        df_clean = clean_data(df_raw)
        if df_clean.empty:
            raise HTTPException(status_code=400, detail="Po wyczyszczeniu danych plik jest pusty.")

        wyniki = process_apartament_data(df_clean)

        # 3. Zapis wyczyszczonego pliku na dysk
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        filepath = f"uploads/cleaned_{timestamp}_{file.filename}"
        df_clean.to_csv(filepath, index=False)

        # 4. Zapis do bazy danych
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute(
            "INSERT INTO logs (name, date, path, stats) VALUES (?, ?, ?, ?)",
            (file.filename, datetime.now().isoformat(), filepath, json.dumps(wyniki))
        )
        conn.commit()
        log_id = cursor.lastrowid  # Pobieramy ID nowo dodanego wpisu
        conn.close()

        return {"id": log_id, "stats": wyniki}

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Błąd przetwarzania: {str(e)}")


@app.get("/logs")
async def get_logs():
    conn = get_db_connection()
    cursor = conn.cursor()
    # Pobieramy historię
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
    row = conn.execute("SELECT path FROM logs WHERE id = ?",(log_id)).fetchone()
    conn.close()

    if not row:
        raise HTTPException(status_code=404,detail='nie znaleziono takiego miast')

    df = pd.read_csv(row['path'])

    analiza = get_city_analytics(df,city_name)

    if not analiza:
        raise HTTPException(status_code=404,detail='nie znaleziono miasta')

    return analiza


# Opcjonalnie: Endpoint do usuwania wpisów
@app.delete("/logs/{log_id}")
async def delete_log(log_id: int):
    conn = get_db_connection()
    cursor = conn.cursor()
    # Najpierw pobierz ścieżkę do pliku, żeby go usunąć z dysku
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


if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)

