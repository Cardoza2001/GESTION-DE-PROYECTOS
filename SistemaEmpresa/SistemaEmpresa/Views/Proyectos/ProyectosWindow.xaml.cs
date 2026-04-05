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
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Proyectos
{
    public sealed partial class ProyectosWindow : Window
    {
        public ProyectosWindow()
        {
            this.InitializeComponent();
            lista.ItemsSource = DataService.Proyectos;
        }

        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                await MostrarMensaje("El nombre no puede estar vacío");
                return;
            }

            // Validar costo
            if (!double.TryParse(txtCosto.Text, out double costo))
            {
                await MostrarMensaje("El costo debe ser un número válido");
                return;
            }

            // Validar negativo
            if (costo < 0)
            {
                await MostrarMensaje("El costo no puede ser negativo");
                return;
            }

            // Crear proyecto
            DataService.Proyectos.Add(new Proyecto
            {
                Nombre = txtNombre.Text,
                CostoEstimado = costo,
                Estado = "Pendiente"
            });

            // Limpiar campos
            txtNombre.Text = "";
            txtCosto.Text = "";
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