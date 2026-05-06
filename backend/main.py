from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import uvicorn
import os
import io
import json
import pandas as pd
from datetime import datetime, timedelta
from contextlib import asynccontextmanager
import bcrypt
import jwt

from data_service import get_city_analytics
from database import get_db_connection, init_db
from data_service import clean_data, process_apartament_data

# JWT config
JWT_SECRET = "super_secret_key"
JWT_ALGORITHM = "HS256"

# MODELE DO AUTH
class RegisterModel(BaseModel):
    username: str
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

@asynccontextmanager
async def lifespan(app: FastAPI):
    if not os.path.exists("uploads"):
        os.makedirs("uploads")
    init_db()
    yield

app = FastAPI(title="Analiza Nieruchomości API", lifespan=lifespan)

# Konfiguracja CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# AUTH ENDPOINTS
@app.post("/register")
async def register(user: RegisterModel):
    conn = get_db_connection()
    cursor = conn.cursor()
    password_hash = bcrypt.hashpw(user.password.encode(), bcrypt.gensalt()).decode()
    try:
        cursor.execute("INSERT INTO users (username, password_hash) VALUES (?, ?)", (user.username, password_hash))
        conn.commit()
    except Exception as e:
        conn.close()
        raise HTTPException(status_code=400, detail="Użytkownik już istnieje lub błąd bazy: " + str(e))
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

# POZOSTAŁE ENDPOINTY
@app.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    if not file.filename.endswith('.csv'):
        raise HTTPException(status_code=400, detail="Tylko pliki CSV są akceptowane.")

    try:
        content = await file.read()
        df_raw = pd.read_csv(io.BytesIO(content))
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

        return {"id": log_id, "stats": wyniki}

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
    row = conn.execute("SELECT path FROM logs WHERE id = ?", (log_id,)).fetchone()
    conn.close()

    if not row:
        raise HTTPException(status_code=404, detail='nie znaleziono takiego miast')

    df = pd.read_csv(row['path'])
    analiza = get_city_analytics(df, city_name)

    if not analiza:
        raise HTTPException(status_code=404, detail='nie znaleziono miasta')

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

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)