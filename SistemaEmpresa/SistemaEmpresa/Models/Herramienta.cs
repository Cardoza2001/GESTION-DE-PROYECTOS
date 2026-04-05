using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Herramienta
    {
        public string? Nombre { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public int Prestadas { get; set; }
        public int Perdidas { get; set; }
    }
}
