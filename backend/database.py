from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import bcrypt
import jwt
import sqlite3
import uvicorn
from datetime import datetime, timedelta

app = FastAPI()

# Database functions

def get_db_connection():
    conn = sqlite3.connect("logs.db")
    conn.row_factory = sqlite3.Row
    return conn

def init_db():
    conn = get_db_connection()
    cursor = conn.cursor()
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS logs(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT,
            date TEXT,
            path TEXT,
            stats JSON
        )
    ''')
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS users(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password_hash TEXT NOT NULL
        )
    ''')
    conn.commit()
    conn.close()


def add_user(username, password):
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # Here you should add code to hash the password before storing it
    
    cursor.execute('''
        INSERT INTO users (username, password_hash)
        VALUES (?, ?)
    ''', (username, password))
    
    conn.commit()
    conn.close()

def query_user(username):
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('''
        SELECT * FROM users WHERE username = ?
    ''', (username,))
    
    user = cursor.fetchone()
    conn.close()
    return user

def remove_user(username):
    conn = get_db_connection()
    cursor = conn.cursor()
    
    cursor.execute('''
        DELETE FROM users WHERE username = ?
    ''', (username,))
    
    conn.commit()
    conn.close()

# Pydantic models

class RegisterModel(BaseModel):
    username: str
    password: str

class LoginModel(BaseModel):
    username: str
    password: str

JWT_SECRET = "super_secret_key"
JWT_ALGORITHM = "HS256"

def create_jwt_token(username: str):
    payload = {
        "sub": username,
        "exp": datetime.utcnow() + timedelta(hours=12)
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

# FastAPI routes

@app.post("/register")
async def register(user: RegisterModel):
    # Registration logic here
    hashed_password = bcrypt.hashpw(user.password.encode('utf-8'), bcrypt.gensalt())
    try:
        add_user(user.username, hashed_password)
    except Exception as e:
        raise HTTPException(status_code=400, detail="Username already exists")
    return {"message": "User registered successfully"}

@app.post("/login")
async def login(user: LoginModel):
    db_user = query_user(user.username)
    if not db_user or not bcrypt.checkpw(user.password.encode('utf-8'), db_user["password_hash"].encode('utf-8')):
        raise HTTPException(status_code=401, detail="Invalid username or password")
    
    # Create JWT token
    token = jwt.encode({"user_id": db_user["id"]}, "SECRET_KEY", algorithm="HS256")
    
    return {"access_token": token, "token_type": "bearer"}

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)





