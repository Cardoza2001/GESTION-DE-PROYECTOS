using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Transaccion
    {
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public double Monto { get; set; }
        public string? Fecha { get; set; }

    }
}