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
using SistemaEmpresa.Views.Consumibles;
using SistemaEmpresa.Views.Herramientas;
using SistemaEmpresa.Views.Proyectos;
using SistemaEmpresa.Views.Jefe;


namespace SistemaEmpresa.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Consumibles_Click(object sender, RoutedEventArgs e)
            => new ConsumiblesWindow().Activate();

        private void Herramientas_Click(object sender, RoutedEventArgs e)
            => new HerramientasWindow().Activate();

        private void Proyectos_Click(object sender, RoutedEventArgs e)
            => new ProyectosWindow().Activate();

        private void Jefe_Click(object sender, RoutedEventArgs e)
            => new JefeWindow().Activate();
    }
}