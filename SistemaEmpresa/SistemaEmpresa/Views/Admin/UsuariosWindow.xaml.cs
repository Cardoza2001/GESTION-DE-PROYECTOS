using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SistemaEmpresa.Database;
using SistemaEmpresa.Models;
using SistemaEmpresa.Services;
using System;
using System.Collections.Generic;

namespace SistemaEmpresa.Views.Admin
{
    public sealed partial class UsuariosWindow : Window
    {
        private Usuario _usuario;

        public UsuariosWindow(Usuario usuario)
        {
            this.InitializeComponent();
            _usuario = usuario;

            this.Activated += (s, e) =>
            {
                try
                {
                    if (e.WindowActivationState != WindowActivationState.Deactivated &&
                        this.Content is FrameworkElement root &&
                        root.XamlRoot != null)
                    {
                        DialogService.Initialize(root.XamlRoot);
                    }
                }
                catch
                {
                }
            };

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
            var lista = new List<Usuario>();

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT U.ID, U.NOMBRE, U.ROL_ID, R.NOMBRE, U.ACTIVO
                FROM USUARIOS U
                JOIN ROLES R ON U.ROL_ID = R.ID
            ";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    RolId = reader.GetInt32(2),
                    Rol = reader.GetString(3),
                    Activo = reader.GetInt32(4) == 1
                });
            }

            listaUsuarios.ItemsSource = lista;
        }

        private async void Crear_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(password) ||
                cmbRol.SelectedValue == null)
            {
                await DialogService.ShowMessage("Completa todos los campos");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var check = connection.CreateCommand();
            check.CommandText = @"
                SELECT COUNT(*) 
                FROM USUARIOS 
                WHERE NOMBRE = @nombre COLLATE NOCASE
            ";
            check.Parameters.AddWithValue("@nombre", nombre);

            long existe = Convert.ToInt64(check.ExecuteScalar());

            if (existe > 0)
            {
                await DialogService.ShowMessage("El usuario ya existe");
                return;
            }

            try
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO USUARIOS (NOMBRE, PASSWORD, ROL_ID, ACTIVO)
                    VALUES (@nombre, @pass, @rol, 1)
                ";

                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@rol", (int)cmbRol.SelectedValue);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    await DialogService.ShowMessage("No se pudo crear el usuario");
                    return;
                }

                await DialogService.ShowMessage("Usuario creado");
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                    await DialogService.ShowMessage("El usuario ya existe");
                else
                    await DialogService.ShowMessage("Error: " + ex.Message);

                return;
            }

            txtNombre.Text = "";
            txtPassword.Password = "";

            CargarUsuarios();
        }

        private async void Desactivar_Click(object sender, RoutedEventArgs e)
        {
            var u = listaUsuarios.SelectedItem as Usuario;

            if (u == null)
            {
                await DialogService.ShowMessage("Selecciona un usuario");
                return;
            }

            if (!u.Activo)
            {
                await DialogService.ShowMessage("El usuario ya está desactivado");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE USUARIOS SET ACTIVO = 0 WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", u.Id);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                await DialogService.ShowMessage("No se pudo actualizar");
                return;
            }

            await DialogService.ShowMessage("Usuario desactivado");

            CargarUsuarios();
        }

        private async void Activar_Click(object sender, RoutedEventArgs e)
        {
            var u = listaUsuarios.SelectedItem as Usuario;

            if (u == null)
            {
                await DialogService.ShowMessage("Selecciona un usuario");
                return;
            }

            if (u.Activo)
            {
                await DialogService.ShowMessage("El usuario ya está activo");
                return;
            }

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE USUARIOS SET ACTIVO = 1 WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", u.Id);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                await DialogService.ShowMessage("No se pudo actualizar");
                return;
            }

            await DialogService.ShowMessage("Usuario activado");

            CargarUsuarios();
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_usuario).Activate();
            this.Close();
        }
    }
}