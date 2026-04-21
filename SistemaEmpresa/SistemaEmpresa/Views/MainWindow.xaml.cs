using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using SistemaEmpresa.Views.Admin;
using SistemaEmpresa.Views.Consumibles;
using SistemaEmpresa.Views.Herramientas;
using SistemaEmpresa.Views.Jefe;
using SistemaEmpresa.Views.Proyectos;
using System;

namespace SistemaEmpresa.Views
{
    public sealed partial class MainWindow : Window
    {
        private Usuario usuarioActual;

        public MainWindow(Usuario usuario)
        {
            this.InitializeComponent();

            usuarioActual = usuario;

            this.Activated += MainWindow_Activated;

            txtBienvenida.Text = $"Bienvenido: {usuarioActual.Nombre} ({usuarioActual.Rol})";

            ConfigurarPermisos();
            CargarDashboard();
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (this.Content is FrameworkElement root)
                DialogService.Initialize(root.XamlRoot);
        }

        private void ConfigurarPermisos()
        {
            if (usuarioActual.Rol == "Empleado")
            {
                btnJefe.Visibility = Visibility.Collapsed;
                btnUsuarios.Visibility = Visibility.Collapsed;
            }

            if (usuarioActual.Rol == "Jefe")
            {
                btnHerramientas.Visibility = Visibility.Collapsed;
                btnUsuarios.Visibility = Visibility.Collapsed;
            }
        }

        private void CargarDashboard()
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
                connection.Open();

                txtTotalConsumibles.Text = ObtenerConteo(connection, "CONSUMIBLES").ToString();
                txtTotalHerramientas.Text = ObtenerConteo(connection, "HERRAMIENTAS").ToString();
                txtTotalProyectos.Text = ObtenerConteo(connection, "PROYECTOS").ToString();
            }
            catch
            {
                txtTotalConsumibles.Text = "0";
                txtTotalHerramientas.Text = "0";
                txtTotalProyectos.Text = "0";
            }
        }

        private int ObtenerConteo(SqliteConnection connection, string tabla)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM {tabla}";

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToInt32(result);
        }

        private void Consumibles_Click(object sender, RoutedEventArgs e)
        {
            new ConsumiblesWindow(usuarioActual).Activate();
            this.Close();
        }

        private void Herramientas_Click(object sender, RoutedEventArgs e)
        {
            new HerramientasWindow(usuarioActual).Activate();
            this.Close();
        }

        private void Proyectos_Click(object sender, RoutedEventArgs e)
        {
            new ProyectosWindow(usuarioActual).Activate();
            this.Close();
        }

        private void Jefe_Click(object sender, RoutedEventArgs e)
        {
            new JefeWindow(usuarioActual).Activate();
            this.Close();
        }

        private void Usuarios_Click(object sender, RoutedEventArgs e)
        {
            new UsuariosWindow(usuarioActual).Activate();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new Login.LoginWindow().Activate();
            this.Close();
        }

        private void MenuHover(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = new SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(255, 224, 242, 254)
                );
            }
        }

        private void MenuLeave(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = new SolidColorBrush(Microsoft.UI.Colors.White);
            }
        }
    }
}