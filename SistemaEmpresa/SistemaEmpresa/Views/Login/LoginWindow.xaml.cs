using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Login
{
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            // Validar campos vacíos
            if (!ValidationService.EsTextoValido(txtUsuario.Text) ||
                !ValidationService.EsTextoValido(txtPassword.Password))
            {
                await MostrarMensaje("Usuario y contraseña son obligatorios");
                return;
            }

            var usuario = DataService.Usuarios.FirstOrDefault(u =>
                u.Nombre == txtUsuario.Text.Trim() &&
                u.Password == txtPassword.Password);

            // Validar credenciales incorrectas
            if (usuario == null)
            {
                await MostrarMensaje("Usuario o contraseña incorrectos");
                return;
            }

            // Login correcto
            MainWindow main = new MainWindow(usuario);
            main.Activate();
            this.Close();
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var dialog = new ContentDialog
            {
                Title = "Login",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = (this.Content as FrameworkElement)!
                            .XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}