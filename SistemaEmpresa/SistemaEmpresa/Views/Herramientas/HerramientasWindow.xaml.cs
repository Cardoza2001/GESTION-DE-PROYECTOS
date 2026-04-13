using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Herramientas
{
    public sealed partial class HerramientasWindow : Window
    {
        private Usuario _usuario; // 👈 ahora usamos usuario

        public HerramientasWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            lista.ItemsSource = DataService.Herramientas;
        }

        private async void Agregar_Click(object sender, RoutedEventArgs e)
        {
            // Validar nombre
            if (!ValidationService.EsTextoValido(txtNombre.Text))
            {
                await MostrarMensaje("El nombre es obligatorio");
                return;
            }

            // Validar cantidad
            if (!ValidationService.EsEnteroValido(txtCantidad.Text, out int cantidad))
            {
                await MostrarMensaje("La cantidad debe ser un número válido");
                return;
            }

            // Validar cantidad mayor a 0
            if (cantidad <= 0)
            {
                await MostrarMensaje("La cantidad debe ser mayor a 0");
                return;
            }

            // Agregar herramienta
            DataService.Herramientas.Add(new Herramienta
            {
                Nombre = txtNombre.Text.Trim(),
                CantidadTotal = cantidad,
                CantidadDisponible = cantidad
            });

            // Limpiar campos
            txtNombre.Text = "";
            txtCantidad.Text = "";
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
            var main = new MainWindow(_usuario); // 👈 recrea MainWindow
            main.Activate();
            this.Close();
        }
    }
}