using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;

namespace SistemaEmpresa.Views.Herramientas
{
    public sealed partial class HerramientasWindow : Window
    {
        private Usuario _usuario;

        public HerramientasWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            this.Activated += (s, e) =>
            {
                if (this.Content is FrameworkElement root)
                {
                    DialogService.Initialize(root.XamlRoot);
                }
            };

            CargarHerramientas();
        }

        private async void CargarHerramientas()
        {
            var listaHerramientas = new List<Herramienta>();

            try
            {
                using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT ID, NOMBRE, CANTIDAD_TOTAL, CANTIDAD_DISPONIBLE, PRESTADAS, PERDIDAS FROM HERRAMIENTAS";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaHerramientas.Add(new Herramienta
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        CantidadTotal = reader.GetInt32(2),
                        CantidadDisponible = reader.GetInt32(3),
                        Prestadas = reader.GetInt32(4),
                        Perdidas = reader.GetInt32(5)
                    });
                }

                lista.ItemsSource = listaHerramientas;
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessage("Error al cargar herramientas:\n" + ex.Message);
            }
        }

        private async void Agregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                await DialogService.ShowMessage("El nombre es obligatorio");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                await DialogService.ShowMessage("Cantidad inválida");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
        SELECT COUNT(*) 
        FROM HERRAMIENTAS 
        WHERE NOMBRE = @nombre COLLATE NOCASE
    ";
            checkCmd.Parameters.AddWithValue("@nombre", nombre);

            long existe = Convert.ToInt64(checkCmd.ExecuteScalar());

            if (existe > 0)
            {
                await DialogService.ShowMessage("La herramienta ya existe");
                return;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
        INSERT INTO HERRAMIENTAS (NOMBRE, CANTIDAD_TOTAL, CANTIDAD_DISPONIBLE)
        VALUES (@nombre, @total, @disponible)
    ";

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@total", cantidad);
            cmd.Parameters.AddWithValue("@disponible", cantidad);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                {
                    await DialogService.ShowMessage("La herramienta ya existe");
                }
                else
                {
                    await DialogService.ShowMessage("Error en base de datos: " + ex.Message);
                }
                return;
            }

            await DialogService.ShowMessage("Herramienta agregada correctamente");

            CargarHerramientas();

            txtNombre.Text = "";
            txtCantidad.Text = "";
        }

        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = lista.SelectedItem as Herramienta;

            if (seleccionada == null)
            {
                await DialogService.ShowMessage("Selecciona una herramienta");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                await DialogService.ShowMessage("Cantidad inválida");
                return;
            }

            if (cantidad > seleccionada.CantidadDisponible)
            {
                await DialogService.ShowMessage("No puedes eliminar más de lo disponible");
                return;
            }

            bool confirmar = await DialogService.Confirm(
                $"¿Eliminar {cantidad} unidad(es) de '{seleccionada.Nombre}'?"
            );

            if (!confirmar) return;

            try
            {
                using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();

                if (seleccionada.CantidadDisponible - cantidad == 0)
                {
                    cmd.CommandText = "DELETE FROM HERRAMIENTAS WHERE ID = @id";
                    cmd.Parameters.AddWithValue("@id", seleccionada.Id);
                }
                else
                {
                    cmd.CommandText = @"
                UPDATE HERRAMIENTAS
                SET 
                    CANTIDAD_DISPONIBLE = CANTIDAD_DISPONIBLE - @cantidad,
                    CANTIDAD_TOTAL = CANTIDAD_TOTAL - @cantidad
                WHERE ID = @id
            ";

                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@id", seleccionada.Id);
                }

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    await DialogService.ShowMessage("No se pudo eliminar");
                    return;
                }

                await DialogService.ShowMessage("Operación realizada");

                CargarHerramientas();
                txtCantidad.Text = "";
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessage("Error: " + ex.Message);
            }
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(_usuario);
            main.Activate();
            this.Close();
        }
    }
}