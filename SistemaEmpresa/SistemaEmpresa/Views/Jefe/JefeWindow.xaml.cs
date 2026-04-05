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

namespace SistemaEmpresa.Views.Jefe
{
    public sealed partial class JefeWindow : Window
    {
        public JefeWindow()
        {
            this.InitializeComponent();

            listaProyectos.ItemsSource = DataService.Proyectos;
            listaTransacciones.ItemsSource = DataService.Transacciones;
        }

        private void Aprobar_Click(object sender, RoutedEventArgs e)
        {
            var p = (Proyecto)listaProyectos.SelectedItem;

            if (p != null)
            {
                if (!DataService.ValidarRecursos())
                    return;

                DataService.AprobarProyecto(p);

                listaProyectos.ItemsSource = null;
                listaProyectos.ItemsSource = DataService.Proyectos;

                listaTransacciones.ItemsSource = null;
                listaTransacciones.ItemsSource = DataService.Transacciones;
            }
        }
    }
}