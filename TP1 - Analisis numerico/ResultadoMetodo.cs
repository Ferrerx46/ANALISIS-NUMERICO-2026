using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class ResultadoMetodo
    {
        public double Raiz { get; set; }
        public int Iteraciones { get; set; }
        public double Error { get; set; }
        public bool Converge { get; set; }
        public string Mensaje { get; set; }

        // Agregamos estos dos para que el gráfico sepa qué dibujar 
        // y el título de la tarjeta se actualice solo
        public string Metodo { get; set; }
        public string Funcion { get; set; }
    }
}
