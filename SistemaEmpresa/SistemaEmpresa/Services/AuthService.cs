using Microsoft.Data.Sqlite;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;

namespace SistemaEmpresa.Services
{
    public static class AuthService
    {
        public static Usuario? Login(string nombre, string password)
        {
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
            SELECT 
                U.ID,
                U.NOMBRE,
                U.PASSWORD,
                U.ROL_ID,
                R.NOMBRE,
                U.ACTIVO
            FROM USUARIOS U
            JOIN ROLES R ON U.ROL_ID = R.ID
            WHERE U.NOMBRE = @nombre
              AND U.PASSWORD = @password
              AND U.ACTIVO = 1
            ";

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Password = reader.GetString(2),
                    RolId = reader.GetInt32(3),
                    Rol = reader.GetString(4),
                    Activo = reader.GetInt32(5) == 1
                };
            }

            return null;
        }
    }
}