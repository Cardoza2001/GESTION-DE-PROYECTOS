using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Usuario
    {
        public string? Nombre { get; set; }
        public string? Rol { get; set; } // Admin, Jefe, Empleado
        public string? Password { get; set; }
    }
}
