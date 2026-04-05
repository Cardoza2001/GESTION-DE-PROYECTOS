using System;
using System.Linq;
using System.Collections.ObjectModel;
using SistemaEmpresa.Models;

namespace SistemaEmpresa.Services
{
    public static class DataService
    {
        public static ObservableCollection<Consumible> Consumibles = new();
        public static ObservableCollection<Herramienta> Herramientas = new();
        public static ObservableCollection<Proyecto> Proyectos = new();
        public static ObservableCollection<Transaccion> Transacciones = new();

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