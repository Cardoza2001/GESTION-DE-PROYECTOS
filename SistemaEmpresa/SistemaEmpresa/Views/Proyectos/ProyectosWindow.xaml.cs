using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Proyectos
{
    public sealed partial class ProyectosWindow : Window
    {
        private Usuario _usuario; // 👈 manejo de sesión

        public ProyectosWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            lista.ItemsSource = DataService.Proyectos;
        }

        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            // Validar nombre
            if (!ValidationService.EsTextoValido(txtNombre.Text))
            {
                await MostrarMensaje("El nombre es obligatorio");
                return;
            }

            // Validar costo
            if (!ValidationService.EsDoubleValido(txtCosto.Text, out double costo))
            {
                await MostrarMensaje("El costo debe ser un número válido");
                return;
            }

            // Validar costo mayor a 0
            if (costo <= 0)
            {
                await MostrarMensaje("El costo debe ser mayor a 0");
                return;
            }

            // Crear proyecto
            DataService.Proyectos.Add(new Proyecto
            {
                Nombre = txtNombre.Text.Trim(),
                CostoEstimado = costo,
                Estado = "Pendiente"
            });

            // Limpiar campos
            txtNombre.Text = "";
            txtCosto.Text = "";
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var root = this.Content as FrameworkElement;

            if (root == null) return;

            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = root.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(_usuario); // 👈 regresar a inicio
            main.Activate();
            this.Close();
        }
    }
}