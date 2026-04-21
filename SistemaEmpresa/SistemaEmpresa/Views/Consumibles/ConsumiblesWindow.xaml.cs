using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using SistemaEmpresa.Database;
using System;
using System.Collections.Generic;

namespace SistemaEmpresa.Views.Consumibles
{
    public sealed partial class ConsumiblesWindow : Window
    {
        private Usuario _usuario;

        public ConsumiblesWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            this.Activated += (s, e) =>
            {
                if (this.Content is FrameworkElement root)
                    DialogService.Initialize(root.XamlRoot);
            };

            CargarConsumibles();
        }

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
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                await DialogService.ShowMessage("El nombre es obligatorio");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad < 0)
            {
                await DialogService.ShowMessage("Cantidad inválida");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var check = connection.CreateCommand();
            check.CommandText = @"
                SELECT COUNT(*) 
                FROM CONSUMIBLES 
                WHERE NOMBRE = @nombre COLLATE NOCASE
            ";
            check.Parameters.AddWithValue("@nombre", nombre);

            long existe = Convert.ToInt64(check.ExecuteScalar());

            if (existe > 0)
            {
                await DialogService.ShowMessage("El consumible ya existe");
                return;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO CONSUMIBLES (NOMBRE, CANTIDAD, NIVEL_MINIMO)
                VALUES (@nombre, @cantidad, 5)
            ";

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                    await DialogService.ShowMessage("El consumible ya existe");
                else
                    await DialogService.ShowMessage("Error: " + ex.Message);

                return;
            }

            await DialogService.ShowMessage("Consumible agregado correctamente");

            CargarConsumibles();

            txtNombre.Text = "";
            txtCantidad.Text = "";
        }

        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = lista.SelectedItem as Consumible;

            if (seleccionado == null)
            {
                await DialogService.ShowMessage("Selecciona un consumible");
                return;
            }

            bool confirmar = await DialogService.Confirm(
                $"¿Eliminar el consumible '{seleccionado.Nombre}'?"
            );

            if (!confirmar)
                return;

            try
            {
                using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM CONSUMIBLES WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", seleccionado.Id);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    await DialogService.ShowMessage("No se pudo eliminar");
                    return;
                }

                await DialogService.ShowMessage("Consumible eliminado");

                CargarConsumibles();
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