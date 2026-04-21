using Microsoft.Data.Sqlite;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using System;

namespace SistemaEmpresa.Services
{
    public static class AuthService
    {
        public static Usuario? Login(string nombre, string password)
        {
            try
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
                    WHERE LOWER(U.NOMBRE) = LOWER(@nombre)
                    LIMIT 1
                ";

                cmd.Parameters.AddWithValue("@nombre", nombre);

                using var reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                // 🔐 DATOS
                string passwordDb = reader.GetString(2);
                bool activo = reader.GetInt32(5) == 1;

                // 🔒 VALIDAR PASSWORD
                if (passwordDb != password)
                    return null;

                // 🚫 VALIDAR ACTIVO
                if (!activo)
                    return null;

                return new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Password = passwordDb,
                    RolId = reader.GetInt32(3),
                    Rol = reader.GetString(4),
                    Activo = activo
                };
            }
            catch (SqliteException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}