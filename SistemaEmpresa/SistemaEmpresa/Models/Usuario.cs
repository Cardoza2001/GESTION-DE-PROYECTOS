using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Usuario
    {
        public int Id { get; set; }             // clave primaria
        public string Nombre { get; set; } = "";
        public string Password { get; set; } = "";

        public int RolId { get; set; }          // relación BD
        public string Rol { get; set; } = "";   // texto (Admin, etc.)

        public bool Activo { get; set; }        // control de acceso
    }
}