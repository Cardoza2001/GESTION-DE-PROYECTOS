using Microsoft.Data.Sqlite;
using System.IO;

namespace SistemaEmpresa.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var directory = Path.GetDirectoryName(DatabaseConfig.DbPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            using var pragma = connection.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            pragma.ExecuteNonQuery();

            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ROLES (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NOMBRE TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS USUARIOS (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NOMBRE TEXT NOT NULL UNIQUE,
                PASSWORD TEXT NOT NULL,
                ROL_ID INTEGER NOT NULL,
                ACTIVO INTEGER DEFAULT 1,
                FOREIGN KEY (ROL_ID) REFERENCES ROLES(ID)
            );

            CREATE TABLE IF NOT EXISTS CONSUMIBLES (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NOMBRE TEXT NOT NULL,
                CANTIDAD INTEGER NOT NULL,
                NIVEL_MINIMO INTEGER DEFAULT 5
            );

            CREATE TABLE IF NOT EXISTS HERRAMIENTAS (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NOMBRE TEXT NOT NULL COLLATE NOCASE UNIQUE,
                CANTIDAD_TOTAL INTEGER NOT NULL,
                CANTIDAD_DISPONIBLE INTEGER NOT NULL,
                PRESTADAS INTEGER DEFAULT 0,
                PERDIDAS INTEGER DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS PROYECTOS (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NOMBRE TEXT NOT NULL,
                COSTO_ESTIMADO REAL NOT NULL,
                ESTADO TEXT DEFAULT 'Pendiente'
            );

            CREATE TABLE IF NOT EXISTS TRANSACCIONES (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                TIPO TEXT NOT NULL,
                DESCRIPCION TEXT,
                MONTO REAL NOT NULL,
                FECHA TEXT DEFAULT CURRENT_TIMESTAMP
            );
            ";

            cmd.ExecuteNonQuery();

            InsertarDatosIniciales(connection);
        }

        private static void InsertarDatosIniciales(SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"
            INSERT OR IGNORE INTO ROLES (ID, NOMBRE) VALUES (1, 'Admin');
            INSERT OR IGNORE INTO ROLES (ID, NOMBRE) VALUES (2, 'Jefe');
            INSERT OR IGNORE INTO ROLES (ID, NOMBRE) VALUES (3, 'Empleado');

            INSERT OR IGNORE INTO USUARIOS (ID, NOMBRE, PASSWORD, ROL_ID, ACTIVO)
            VALUES (1, 'admin', '123', 1, 1);
            ";

            cmd.ExecuteNonQuery();
        }
    }
}