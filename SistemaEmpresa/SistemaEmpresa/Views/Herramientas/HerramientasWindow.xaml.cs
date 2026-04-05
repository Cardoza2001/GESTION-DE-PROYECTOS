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

namespace SistemaEmpresa.Views.Herramientas
{
    public sealed partial class HerramientasWindow : Window
    {
        public HerramientasWindow()
        {
            this.InitializeComponent();
            lista.ItemsSource = DataService.Herramientas;
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = int.Parse(txtCantidad.Text);

            DataService.Herramientas.Add(new Herramienta
            {
                Nombre = txtNombre.Text,
                CantidadTotal = cantidad,
                CantidadDisponible = cantidad
            });

            lista.ItemsSource = null;
            lista.ItemsSource = DataService.Herramientas;
        }
    }
}
