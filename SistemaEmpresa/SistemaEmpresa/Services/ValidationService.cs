using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEmpresa.Services
{
    public static class ValidationService
    {
        public static bool EsTextoValido(string? texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }

        public static bool EsEnteroValido(string? texto, out int valor)
        {
            return int.TryParse(texto, out valor);
        }

        public static bool EsDoubleValido(string? texto, out double valor)
        {
            return double.TryParse(texto, out valor);
        }
    }
}
