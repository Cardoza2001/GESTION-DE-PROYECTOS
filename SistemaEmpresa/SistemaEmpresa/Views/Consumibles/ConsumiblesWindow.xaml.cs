using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SistemaEmpresa.Views.Consumibles
{
    public sealed partial class ConsumiblesWindow : Window
    {
        public ConsumiblesWindow()
        {
            this.InitializeComponent();

            // 🔥 SOLO se asigna una vez
            lista.ItemsSource = DataService.Consumibles;
        }

        private async void Agregar_Click(object sender, RoutedEventArgs e)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                await MostrarMensaje("El nombre no puede estar vacío");
                return;
            }

            // Validar cantidad
            if (!int.TryParse(txtCantidad.Text, out int cantidad))
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
                Nombre = txtNombre.Text,
                Cantidad = cantidad,
                NivelMinimo = 5
            });

            // 🔥 Limpiar campos (opcional pero recomendado)
            txtNombre.Text = "";
            txtCantidad.Text = "";
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}