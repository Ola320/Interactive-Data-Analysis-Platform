import os
import sqlite3

DB_FILENAME = 'logs.db'
DB_PATH = os.path.join(os.path.dirname(__file__), DB_FILENAME)


def get_db_connection():
    conn = sqlite3.connect(DB_PATH)
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
            email TEXT UNIQUE,
            password_hash TEXT NOT NULL
        )
    ''')

    cursor.execute("PRAGMA table_info(users)")
    existing_columns = [row['name'] for row in cursor.fetchall()]
    if 'email' not in existing_columns:
        cursor.execute('ALTER TABLE users ADD COLUMN email TEXT')
        cursor.execute('CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email)')

    conn.commit()
    conn.close()
