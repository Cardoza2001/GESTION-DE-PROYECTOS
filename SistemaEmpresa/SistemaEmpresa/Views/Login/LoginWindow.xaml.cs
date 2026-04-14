using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
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
            // 🔐 Validar campos vacíos
            if (!ValidationService.EsTextoValido(txtUsuario.Text) ||
                !ValidationService.EsTextoValido(txtPassword.Password))
            {
                await MostrarMensaje("Usuario y contraseña son obligatorios");
                return;
            }

            // 🔥 LOGIN REAL CON SQLITE
            var usuario = AuthService.Login(
                txtUsuario.Text.Trim(),
                txtPassword.Password
            );

            // ❌ Credenciales incorrectas
            if (usuario == null)
            {
                await MostrarMensaje("Usuario o contraseña incorrectos o usuario inactivo");
                return;
            }

            // ✅ Login correcto
            MainWindow main = new MainWindow(usuario);
            main.Activate();
            this.Close();
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var root = this.Content as FrameworkElement;

            if (root == null) return;

            var dialog = new ContentDialog
            {
                Title = "Login",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = root.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}