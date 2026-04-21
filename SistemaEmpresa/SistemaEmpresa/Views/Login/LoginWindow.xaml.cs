using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;

namespace SistemaEmpresa.Views.Login
{
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();
            this.Activated += LoginWindow_Activated;
        }

        private void LoginWindow_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (this.Content is FrameworkElement root)
                DialogService.Initialize(root.XamlRoot);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationService.EsTextoValido(txtUsuario.Text) ||
                !ValidationService.EsTextoValido(txtPassword.Password))
            {
                await DialogService.ShowMessage("Usuario y contraseña son obligatorios");
                return;
            }

            var usuario = AuthService.Login(
                txtUsuario.Text.Trim(),
                txtPassword.Password
            );

            if (usuario == null)
            {
                await DialogService.ShowMessage("Usuario o contraseña incorrectos o usuario inactivo");
                return;
            }

            new MainWindow(usuario).Activate();
            this.Close();
        }
    }
}