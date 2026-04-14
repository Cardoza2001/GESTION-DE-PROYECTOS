using Microsoft.Data.Sqlite;
using SistemaEmpresa.Database;

namespace SistemaEmpresa.Services
{
    public static class UserService
    {
        public static void CrearUsuario(string nombre, string password, int rolId)
        {
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
            INSERT INTO USUARIOS (NOMBRE, PASSWORD, ROL_ID, ACTIVO)
            VALUES (@nombre, @password, @rol, 1)
            ";

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@rol", rolId);

            cmd.ExecuteNonQuery();
        }

        public static void DesactivarUsuario(int id)
        {
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText = "UPDATE USUARIOS SET ACTIVO = 0 WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
    }
}