using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Views.Consumibles;
using SistemaEmpresa.Views.Herramientas;
using SistemaEmpresa.Views.Jefe;
using SistemaEmpresa.Views.Proyectos;
using SistemaEmpresa.Models;

namespace SistemaEmpresa.Views
{
    public sealed partial class MainWindow : Window
    {
        private Usuario usuarioActual;

        public MainWindow(Usuario usuario)
        {
            this.InitializeComponent();

            usuarioActual = usuario;

            txtBienvenida.Text = $"Bienvenido: {usuarioActual.Nombre} ({usuarioActual.Rol})";

            ConfigurarPermisos();

        }

        private void ConfigurarPermisos()
        {
            if (usuarioActual.Rol == "Empleado")
            {
                btnJefe.Visibility = Visibility.Collapsed;
            }

            if (usuarioActual.Rol == "Jefe")
            {
                btnHerramientas.Visibility = Visibility.Collapsed;
            }
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new SistemaEmpresa.Views.Login.LoginWindow();
            login.Activate();
            this.Close();
        }
    }
}