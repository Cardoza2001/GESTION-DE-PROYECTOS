using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SistemaEmpresa.Views.Consumibles;
using SistemaEmpresa.Views.Herramientas;
using SistemaEmpresa.Views.Jefe;
using SistemaEmpresa.Views.Proyectos;
using SistemaEmpresa.Views.Admin;
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
            // 🔒 Empleado
            if (usuarioActual.Rol == "Empleado")
            {
                btnJefe.Visibility = Visibility.Collapsed;
                btnUsuarios.Visibility = Visibility.Collapsed;
            }

            // 🔧 Jefe
            if (usuarioActual.Rol == "Jefe")
            {
                btnHerramientas.Visibility = Visibility.Collapsed;
                btnUsuarios.Visibility = Visibility.Collapsed;
            }

            // 👑 Admin → acceso total
        }

        // ===============================
        // 🔥 NAVEGACIÓN
        // ===============================

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
            var login = new SistemaEmpresa.Views.Login.LoginWindow();
            login.Activate();
            this.Close();
        }

        // ===============================
        // 🎨 HOVER EFECTO MENÚ
        // ===============================

        private void MenuHover(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 224, 242, 254)); // azul claro
            }
        }

        private void MenuLeave(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = new SolidColorBrush(
                    Microsoft.UI.Colors.White
                );
            }
        }
    }
}