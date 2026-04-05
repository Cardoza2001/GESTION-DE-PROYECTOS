using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Consumible
    {
        public string? Nombre { get; set; }
        public int Cantidad { get; set; }
        public int NivelMinimo { get; set; }
    }
}
