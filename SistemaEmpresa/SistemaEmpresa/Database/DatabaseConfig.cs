using System;
using System.IO;

namespace SistemaEmpresa.Database
{
    public static class DatabaseConfig
    {
        public static string DbPath =>
            Path.Combine(AppContext.BaseDirectory, "Database", "database.db");

        public static string ConnectionString =>
            $"Data Source={DbPath}";
    }
}