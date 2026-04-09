using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SistemaEmpresa.Services;
using SistemaEmpresa.Models;

namespace SistemaEmpresa.Views.Login
{
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var usuario = DataService.Usuarios.FirstOrDefault(u =>
                u.Nombre == txtUsuario.Text &&
                u.Password == txtPassword.Password);

            if (usuario != null)
            {
                MainWindow main = new MainWindow(usuario);
                main.Activate();
                this.Close();
            }
        }
    }
}