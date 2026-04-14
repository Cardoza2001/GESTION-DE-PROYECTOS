using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Herramientas
{
    public sealed partial class HerramientasWindow : Window
    {
        private Usuario _usuario;

        public HerramientasWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            CargarHerramientas();
        }

        // 🔥 CARGAR DESDE SQLITE
        private void CargarHerramientas()
        {
            var listaHerramientas = new List<Herramienta>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ID, NOMBRE, CANTIDAD_TOTAL, CANTIDAD_DISPONIBLE FROM HERRAMIENTAS";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                listaHerramientas.Add(new Herramienta
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    CantidadTotal = reader.GetInt32(2),
                    CantidadDisponible = reader.GetInt32(3)
                });
            }

            lista.ItemsSource = listaHerramientas;
        }

        private async void Agregar_Click(object sender, RoutedEventArgs e)
        {
            // 🔐 Validar nombre
            if (!ValidationService.EsTextoValido(txtNombre.Text))
            {
                await MostrarMensaje("El nombre es obligatorio");
                return;
            }

            // 🔢 Validar cantidad
            if (!ValidationService.EsEnteroValido(txtCantidad.Text, out int cantidad))
            {
                await MostrarMensaje("La cantidad debe ser un número válido");
                return;
            }

            // 🚫 Validar mayor a 0
            if (cantidad <= 0)
            {
                await MostrarMensaje("La cantidad debe ser mayor a 0");
                return;
            }

            // 💾 INSERT EN SQLITE
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO HERRAMIENTAS (NOMBRE, CANTIDAD_TOTAL, CANTIDAD_DISPONIBLE)
                VALUES (@nombre, @total, @disponible)
            ";

            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@total", cantidad);
            cmd.Parameters.AddWithValue("@disponible", cantidad);

            cmd.ExecuteNonQuery();

            // 🔄 RECARGAR LISTA
            CargarHerramientas();

            // 🧹 Limpiar campos
            txtNombre.Text = "";
            txtCantidad.Text = "";
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