using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEmpresa.Views.Admin
{
    public sealed partial class UsuariosWindow : Window
    {
        private Usuario _usuario;

        public UsuariosWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            CargarRoles();
            CargarUsuarios();
        }

        private void CargarRoles()
        {
            var roles = new Dictionary<int, string>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ID, NOMBRE FROM ROLES";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(reader.GetInt32(0), reader.GetString(1));
            }

            cmbRol.ItemsSource = roles;
            cmbRol.DisplayMemberPath = "Value";
            cmbRol.SelectedValuePath = "Key";
        }

        private void CargarUsuarios()
        {
            var lista = new List<string>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT U.ID, U.NOMBRE, R.NOMBRE, U.ACTIVO
                FROM USUARIOS U
                JOIN ROLES R ON U.ROL_ID = R.ID
            ";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string estado = reader.GetInt32(3) == 1 ? "Activo" : "Inactivo";

                lista.Add($"{reader.GetInt32(0)} | {reader.GetString(1)} | {reader.GetString(2)} | {estado}");
            }

            listaUsuarios.ItemsSource = lista;
        }

        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password) ||
                cmbRol.SelectedValue == null)
            {
                await MostrarMensaje("Completa todos los campos");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO USUARIOS (NOMBRE, PASSWORD, ROL_ID, ACTIVO)
                VALUES (@nombre, @pass, @rol, 1)
            ";

            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@pass", txtPassword.Password);
            cmd.Parameters.AddWithValue("@rol", (int)cmbRol.SelectedValue);

            cmd.ExecuteNonQuery();

            await MostrarMensaje("Usuario creado");

            txtNombre.Text = "";
            txtPassword.Password = "";

            CargarUsuarios();
        }

        private async void Desactivar_Click(object sender, RoutedEventArgs e)
        {
            if (listaUsuarios.SelectedItem == null)
            {
                await MostrarMensaje("Selecciona un usuario");
                return;
            }

            var texto = listaUsuarios.SelectedItem.ToString();
            int id = int.Parse(texto.Split('|')[0]);

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE USUARIOS SET ACTIVO = 0 WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            await MostrarMensaje("Usuario desactivado");

            CargarUsuarios();
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
            var main = new MainWindow(_usuario);
            main.Activate();
            this.Close();
        }
    }
}