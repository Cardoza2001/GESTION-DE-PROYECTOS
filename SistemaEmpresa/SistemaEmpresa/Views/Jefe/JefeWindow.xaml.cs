using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Jefe
{
    public sealed partial class JefeWindow : Window
    {
        private Usuario _usuario; // 👈 manejo de sesión

        public JefeWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            listaProyectos.ItemsSource = DataService.Proyectos;
            listaTransacciones.ItemsSource = DataService.Transacciones;
        }

        private async void Aprobar_Click(object sender, RoutedEventArgs e)
        {
            var p = listaProyectos.SelectedItem as Proyecto;

            // Validar selección
            if (p == null)
            {
                await MostrarMensaje("Selecciona un proyecto");
                return;
            }

            // Validar si ya está aprobado
            if (p.Estado == "Aprobado")
            {
                await MostrarMensaje("El proyecto ya fue aprobado");
                return;
            }

            // Validar recursos
            if (!DataService.ValidarRecursos())
            {
                await MostrarMensaje("No hay herramientas suficientes para aprobar el proyecto");
                return;
            }

            // Aprobar proyecto
            DataService.AprobarProyecto(p);

            await MostrarMensaje("Proyecto aprobado correctamente");
        }

        private async Task MostrarMensaje(string mensaje)
        {
            var root = this.Content as FrameworkElement;

            if (root == null) return;

            var dialog = new ContentDialog
            {
                Title = "Sistema",
                Content = mensaje,
                CloseButtonText = "OK",
                XamlRoot = root.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(_usuario); // 👈 regresar
            main.Activate();
            this.Close();
        }
    }
}