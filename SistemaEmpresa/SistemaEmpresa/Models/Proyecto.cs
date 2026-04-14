using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Area { get; set; }
        public string? Estado { get; set; }
        public string? Descripcion { get; set; }
        public double CostoEstimado { get; set; }
    }
}
