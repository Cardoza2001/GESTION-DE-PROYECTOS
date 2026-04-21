using System;

namespace SistemaEmpresa.Models
{
    public class Usuario
    {
        // 🔑 PRIMARY KEY
        public int Id { get; set; }

        // 👤 DATOS
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // 🔗 RELACIÓN CON ROLES
        public int RolId { get; set; }

        // 🏷️ TEXTO DEL ROL (JOIN)
        public string Rol { get; set; } = string.Empty;

        // 🔒 CONTROL DE ACCESO
        public bool Activo { get; set; }

        // 🎨 PROPIEDADES CALCULADAS (UI)
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";

        public string EstadoColor => Activo ? "#10B981" : "#EF4444";
    }
}