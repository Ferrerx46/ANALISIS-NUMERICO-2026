using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculus;
using System.Text.RegularExpressions;

namespace TP1___Analisis_numerico
{
    public class EvaluadorFuncion
    {
        private Calculo analizador;
        private string funcionLimpia; // Nota: tenés declarada esta variable pero no la estabas usando

        public EvaluadorFuncion(string funcion)
        {
            analizador = new Calculo();

            // 1. Limpieza básica
            string limpia = funcion.Replace(" ", "")
                                   .ToLower()
                                   .Replace(",", ".")
                                   .Replace("−", "-");

            // 2. Expresión regular para agregar '*' entre un número y la 'x'
            // Busca cualquier dígito (\d) seguido inmediatamente por una 'x' y le mete un '*' en el medio
            limpia = Regex.Replace(limpia, @"(\d)(x)", "$1*$2");

            this.funcionLimpia = limpia; // Guardamos por las dudas

            // 3. Verificación de sintaxis
            if (!analizador.Sintaxis(limpia, 'x'))
                throw new Exception("Error de sintaxis evaluando: " + limpia);
        }

        public double Evaluar(double x)
        {
            double val = analizador.EvaluaFx(x);

            if (double.IsNaN(val) || double.IsInfinity(val))
                throw new Exception("Error evaluando la función en x = " + x);

            return val;
        }

        public double Derivada(double x)
        {
            return DerivadaNumerica(x);
        }

        private double DerivadaNumerica(double x)
        {
            double h = 1e-6;
            return (Evaluar(x + h) - Evaluar(x - h)) / (2 * h);
        }
    }
}
