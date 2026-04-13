using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Consumibles
{
    public sealed partial class ConsumiblesWindow : Window
    {
        private Usuario _usuario; // 👈 ahora guardamos el usuario

        public ConsumiblesWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            lista.ItemsSource = DataService.Consumibles;
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

            // Validar negativo
            if (cantidad < 0)
            {
                await MostrarMensaje("La cantidad no puede ser negativa");
                return;
            }

            // Agregar consumible
            DataService.Consumibles.Add(new Consumible
            {
                Nombre = txtNombre.Text.Trim(),
                Cantidad = cantidad,
                NivelMinimo = 5
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
            var main = new MainWindow(_usuario); // 👈 recrea la principal
            main.Activate();
            this.Close();
        }
    }
}