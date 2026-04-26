import sqlite3


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
    conn.commit()
    conn.close()



