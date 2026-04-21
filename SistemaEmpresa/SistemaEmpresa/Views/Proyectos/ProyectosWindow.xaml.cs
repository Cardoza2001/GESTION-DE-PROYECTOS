using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;

namespace SistemaEmpresa.Views.Proyectos
{
    public sealed partial class ProyectosWindow : Window
    {
        private Usuario _usuario;

        public ProyectosWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            this.Activated += (s, e) =>
            {
                if (this.Content is FrameworkElement root)
                    DialogService.Initialize(root.XamlRoot);
            };

            CargarProyectos();
        }

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
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                await DialogService.ShowMessage("El nombre es obligatorio");
                return;
            }

            if (!double.TryParse(txtCosto.Text, out double costo) || costo <= 0)
            {
                await DialogService.ShowMessage("El costo debe ser válido y mayor a 0");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var check = connection.CreateCommand();
            check.CommandText = @"
                SELECT COUNT(*) 
                FROM PROYECTOS 
                WHERE NOMBRE = @nombre COLLATE NOCASE
            ";
            check.Parameters.AddWithValue("@nombre", nombre);

            long existe = Convert.ToInt64(check.ExecuteScalar());

            if (existe > 0)
            {
                await DialogService.ShowMessage("El proyecto ya existe");
                return;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO PROYECTOS (NOMBRE, COSTO_ESTIMADO, ESTADO)
                VALUES (@nombre, @costo, 'Pendiente')
            ";

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@costo", costo);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                    await DialogService.ShowMessage("El proyecto ya existe");
                else
                    await DialogService.ShowMessage("Error: " + ex.Message);

                return;
            }

            await DialogService.ShowMessage("Proyecto creado correctamente");

            CargarProyectos();

            txtNombre.Text = "";
            txtCosto.Text = "";
        }

        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var proyecto = lista.SelectedItem as Proyecto;

            if (proyecto == null)
            {
                await DialogService.ShowMessage("Selecciona un proyecto");
                return;
            }

            if (proyecto.Estado != "Rechazado")
            {
                await DialogService.ShowMessage("Solo se pueden eliminar proyectos rechazados");
                return;
            }

            bool confirmar = await DialogService.Confirm(
                $"¿Eliminar el proyecto '{proyecto.Nombre}'?"
            );

            if (!confirmar)
                return;

            try
            {
                using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM PROYECTOS WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", proyecto.Id);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    await DialogService.ShowMessage("No se pudo eliminar");
                    return;
                }

                await DialogService.ShowMessage("Proyecto eliminado");

                CargarProyectos();
            }
            catch (SqliteException ex)
            {
                await DialogService.ShowMessage("Error: " + ex.Message);
            }
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_usuario).Activate();
            this.Close();
        }
    }
}