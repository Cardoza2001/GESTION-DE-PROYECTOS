using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using SistemaEmpresa.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Consumibles
{
    public sealed partial class ConsumiblesWindow : Window
    {
        private Usuario _usuario;

        public ConsumiblesWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            CargarConsumibles();
        }

        // 🔥 CARGAR DESDE SQLITE
        private void CargarConsumibles()
        {
            var listaConsumibles = new List<Consumible>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ID, NOMBRE, CANTIDAD, NIVEL_MINIMO FROM CONSUMIBLES";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                listaConsumibles.Add(new Consumible
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Cantidad = reader.GetInt32(2),
                    NivelMinimo = reader.GetInt32(3)
                });
            }

            lista.ItemsSource = listaConsumibles;
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

            // 🚫 Validar negativo
            if (cantidad < 0)
            {
                await MostrarMensaje("La cantidad no puede ser negativa");
                return;
            }

            // 💾 INSERT EN SQLITE
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO CONSUMIBLES (NOMBRE, CANTIDAD, NIVEL_MINIMO)
                VALUES (@nombre, @cantidad, 5)
            ";

            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@cantidad", cantidad);

            cmd.ExecuteNonQuery();

            // 🔄 RECARGAR LISTA
            CargarConsumibles();

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