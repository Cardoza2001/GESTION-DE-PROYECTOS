using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Jefe
{
    public sealed partial class JefeWindow : Window
    {
        private Usuario _usuario;

        public JefeWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            CargarProyectos();
            CargarTransacciones();
        }

        // 🔥 CARGAR PROYECTOS
        private void CargarProyectos()
        {
            var lista = new List<Proyecto>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ID, NOMBRE, COSTO_ESTIMADO, ESTADO FROM PROYECTOS";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Proyecto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    CostoEstimado = reader.GetDouble(2),
                    Estado = reader.GetString(3)
                });
            }

            listaProyectos.ItemsSource = lista;
        }

        // 🔥 CARGAR TRANSACCIONES
        private void CargarTransacciones()
        {
            var lista = new List<string>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TIPO, DESCRIPCION, MONTO, FECHA FROM TRANSACCIONES";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add($"{reader.GetString(0)} | {reader.GetString(1)} | ${reader.GetDouble(2)} | {reader.GetString(3)}");
            }

            listaTransacciones.ItemsSource = lista;
        }

        private async void Aprobar_Click(object sender, RoutedEventArgs e)
        {
            var p = listaProyectos.SelectedItem as Proyecto;

            // 🔐 Validar selección
            if (p == null)
            {
                await MostrarMensaje("Selecciona un proyecto");
                return;
            }

            // 🚫 Ya aprobado
            if (p.Estado == "Aprobado")
            {
                await MostrarMensaje("El proyecto ya fue aprobado");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            // 🔧 VALIDAR RECURSOS (simple: herramientas disponibles > 0)
            var validarCmd = connection.CreateCommand();
            validarCmd.CommandText = "SELECT SUM(CANTIDAD_DISPONIBLE) FROM HERRAMIENTAS";

            var total = validarCmd.ExecuteScalar();

            if (total == null || total == DBNull.Value || (long)total <= 0)
            {
                await MostrarMensaje("No hay herramientas disponibles");
                return;
            }

            // ✅ APROBAR PROYECTO
            var aprobarCmd = connection.CreateCommand();
            aprobarCmd.CommandText = @"
                UPDATE PROYECTOS
                SET ESTADO = 'Aprobado'
                WHERE ID = @id
            ";

            aprobarCmd.Parameters.AddWithValue("@id", p.Id);
            aprobarCmd.ExecuteNonQuery();

            // 💸 REGISTRAR TRANSACCIÓN
            var transCmd = connection.CreateCommand();
            transCmd.CommandText = @"
                INSERT INTO TRANSACCIONES (TIPO, DESCRIPCION, MONTO)
                VALUES ('Gasto', @desc, @monto)
            ";

            transCmd.Parameters.AddWithValue("@desc", "Aprobación de proyecto: " + p.Nombre);
            transCmd.Parameters.AddWithValue("@monto", p.CostoEstimado);

            transCmd.ExecuteNonQuery();

            await MostrarMensaje("Proyecto aprobado correctamente");

            // 🔄 RECARGAR DATOS
            CargarProyectos();
            CargarTransacciones();
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var root = this.Content as FrameworkElement;
            if (root == null) return;

            var dialog = new ContentDialog
            {
                Title = "Sistema",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = root.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(_usuario);
            main.Activate();
            this.Close();
        }
    }
}