using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace SistemaEmpresa.Views.Jefe
{
    public sealed partial class JefeWindow : Window
    {
        private Usuario _usuario;

        public JefeWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            this.Activated += (s, e) =>
            {
                if (this.Content is FrameworkElement root)
                    DialogService.Initialize(root.XamlRoot);
            };

            CargarProyectos();
            CargarTransacciones();
        }

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

        private void CargarTransacciones()
        {
            var lista = new List<Transaccion>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TIPO, DESCRIPCION, MONTO, FECHA 
                FROM TRANSACCIONES 
                ORDER BY ID DESC
            ";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Transaccion
                {
                    Tipo = reader.GetString(0),
                    Descripcion = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Monto = reader.GetDouble(2),
                    Fecha = reader.GetString(3)
                });
            }

            listaTransacciones.ItemsSource = lista;
        }

        private async void Aprobar_Click(object sender, RoutedEventArgs e)
        {
            var p = listaProyectos.SelectedItem as Proyecto;

            if (p == null)
            {
                await DialogService.ShowMessage("Selecciona un proyecto");
                return;
            }

            if (p.Estado == "Aprobado")
            {
                await DialogService.ShowMessage("El proyecto ya está aprobado");
                return;
            }

            bool confirmar = await DialogService.Confirm($"¿Aprobar el proyecto '{p.Nombre}'?");
            if (!confirmar) return;

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var validar = connection.CreateCommand();
            validar.CommandText = "SELECT SUM(CANTIDAD_DISPONIBLE) FROM HERRAMIENTAS";

            var total = validar.ExecuteScalar();

            if (total == null || total == DBNull.Value || Convert.ToInt64(total) <= 0)
            {
                await DialogService.ShowMessage("No hay herramientas disponibles");
                return;
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                var update = connection.CreateCommand();
                update.Transaction = transaction;
                update.CommandText = "UPDATE PROYECTOS SET ESTADO = 'Aprobado' WHERE ID = @id";
                update.Parameters.AddWithValue("@id", p.Id);

                int rows = update.ExecuteNonQuery();

                if (rows == 0)
                {
                    await DialogService.ShowMessage("No se pudo actualizar el proyecto");
                    transaction.Rollback();
                    return;
                }

                var trans = connection.CreateCommand();
                trans.Transaction = transaction;
                trans.CommandText = @"
                    INSERT INTO TRANSACCIONES (TIPO, DESCRIPCION, MONTO)
                    VALUES ('Egreso', @desc, @monto)
                ";

                trans.Parameters.AddWithValue("@desc", $"Proyecto aprobado: {p.Nombre}");
                trans.Parameters.AddWithValue("@monto", p.CostoEstimado);

                trans.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                await DialogService.ShowMessage("Error: " + ex.Message);
                return;
            }

            await DialogService.ShowMessage("Proyecto aprobado");

            CargarProyectos();
            CargarTransacciones();
        }

        private async void Rechazar_Click(object sender, RoutedEventArgs e)
        {
            var p = listaProyectos.SelectedItem as Proyecto;

            if (p == null)
            {
                await DialogService.ShowMessage("Selecciona un proyecto");
                return;
            }

            if (p.Estado == "Rechazado")
            {
                await DialogService.ShowMessage("Ya está rechazado");
                return;
            }

            bool confirmar = await DialogService.Confirm($"¿Rechazar el proyecto '{p.Nombre}'?");
            if (!confirmar) return;

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE PROYECTOS SET ESTADO = 'Rechazado' WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", p.Id);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                await DialogService.ShowMessage("No se pudo actualizar el proyecto");
                return;
            }

            await DialogService.ShowMessage("Proyecto rechazado");

            CargarProyectos();
        }

        private async void GenerarPDF_Click(object sender, RoutedEventArgs e)
        {
            var proyecto = listaProyectos.SelectedItem as Proyecto;

            if (proyecto == null)
            {
                await DialogService.ShowMessage("Selecciona un proyecto");
                return;
            }

            var picker = new FileSavePicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));

            picker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });
            picker.SuggestedFileName = $"Reporte_{proyecto.Nombre}";

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            var transacciones = new List<Transaccion>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TIPO, DESCRIPCION, MONTO, FECHA
                FROM TRANSACCIONES
                WHERE DESCRIPCION LIKE @nombre
            ";

            cmd.Parameters.AddWithValue("@nombre", "%" + proyecto.Nombre + "%");

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                transacciones.Add(new Transaccion
                {
                    Tipo = reader.GetString(0),
                    Descripcion = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Monto = reader.GetDouble(2),
                    Fecha = reader.GetString(3)
                });
            }

            using var stream = await file.OpenStreamForWriteAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text("REPORTE DE PROYECTO")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Proyecto: {proyecto.Nombre}");
                        col.Item().Text($"Costo: ${proyecto.CostoEstimado}");
                        col.Item().Text($"Estado: {proyecto.Estado}");
                        col.Item().Text($"Fecha: {DateTime.Now}");

                        col.Item().LineHorizontal(1);
                        col.Item().Text("Transacciones:").Bold();

                        foreach (var t in transacciones)
                        {
                            col.Item().Text($"• {t.Tipo} | {t.Descripcion} | ${t.Monto} | {t.Fecha}");
                        }
                    });
                });
            }).GeneratePdf(stream);

            await DialogService.ShowMessage("PDF generado correctamente");
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_usuario).Activate();
            this.Close();
        }
    }
}