using SistemaEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaEmpresa.Services
{
    public static class DataService
    {
        public static ObservableCollection<Consumible> Consumibles { get; } = new();
        public static ObservableCollection<Herramienta> Herramientas { get; } = new();
        public static ObservableCollection<Proyecto> Proyectos { get; } = new();
        public static ObservableCollection<Transaccion> Transacciones { get; } = new();

        // USUARIOS DEL SISTEMA
        public static List<Usuario> Usuarios { get; } = new()
        {
            new Usuario { Nombre = "admin", Password = "123", Rol = "Admin" },
            new Usuario { Nombre = "jefe", Password = "123", Rol = "Jefe" },
            new Usuario { Nombre = "empleado", Password = "123", Rol = "Empleado" }
        };

        // VALIDAR RECURSOS
        public static bool ValidarRecursos()
        {
            return Herramientas.All(h => h.CantidadDisponible > 0);
        }

        // APROBAR PROYECTO
        public static void AprobarProyecto(Proyecto p)
        {
            p.Estado = "Aprobado";

            Transacciones.Add(new Transaccion
            {
                Tipo = "Egreso",
                Descripcion = $"Proyecto aprobado: {p.Nombre}",
                Monto = p.CostoEstimado,
                Fecha = DateTime.Now.ToShortDateString()
            });
        }

        // ALERTA CONSUMIBLES
        public static string VerificarConsumibles()
        {
            foreach (var c in Consumibles)
            {
                if (c.Cantidad <= c.NivelMinimo)
                    return $"Alerta: {c.Nombre} bajo";
            }

            return "";
        }
    }
}