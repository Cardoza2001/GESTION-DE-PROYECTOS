using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Proyectos
{
    public sealed partial class ProyectosWindow : Window
    {
        private Usuario _usuario;

        public ProyectosWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            CargarProyectos();
        }

        // 🔥 CARGAR DESDE SQLITE
        private void CargarProyectos()
        {
            var listaProyectos = new List<Proyecto>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ID, NOMBRE, COSTO_ESTIMADO, ESTADO FROM PROYECTOS";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                listaProyectos.Add(new Proyecto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    CostoEstimado = reader.GetDouble(2),
                    Estado = reader.GetString(3)
                });
            }

            lista.ItemsSource = listaProyectos;
        }

        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            // 🔐 Validar nombre
            if (!ValidationService.EsTextoValido(txtNombre.Text))
            {
                await MostrarMensaje("El nombre es obligatorio");
                return;
            }

            // 🔢 Validar costo
            if (!ValidationService.EsDoubleValido(txtCosto.Text, out double costo))
            {
                await MostrarMensaje("El costo debe ser un número válido");
                return;
            }

            // 🚫 Validar > 0
            if (costo <= 0)
            {
                await MostrarMensaje("El costo debe ser mayor a 0");
                return;
            }

            // 💾 INSERT EN SQLITE
            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO PROYECTOS (NOMBRE, COSTO_ESTIMADO, ESTADO)
                VALUES (@nombre, @costo, 'Pendiente')
            ";

            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@costo", costo);

            cmd.ExecuteNonQuery();

            // 🔄 RECARGAR LISTA
            CargarProyectos();

            // 🧹 Limpiar campos
            txtNombre.Text = "";
            txtCosto.Text = "";
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