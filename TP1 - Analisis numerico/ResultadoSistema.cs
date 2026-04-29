using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class ResultadoSistema
    {
        public double[] Soluciones { get; set; } // Un array para x1, x2, x3...
        public int Iteraciones { get; set; }
        public double Error { get; set; } // En Gauss-Seidel podrías usar el error máximo
        public bool Converge { get; set; }
        public string Mensaje { get; set; }
        public string Metodo { get; set; }
    }
}
