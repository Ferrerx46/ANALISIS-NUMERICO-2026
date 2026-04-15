using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class MetodosCerrados
    {
        public ResultadoMetodo Resolver(string funcionStr, double xi, double xd, double tolerancia, int iteraciones, string metodo)
        {
            var f = new EvaluadorFuncion(funcionStr);
            var resultado = new ResultadoMetodo();

            // Guardamos las evaluaciones iniciales
            double fxi = f.Evaluar(xi);
            double fxd = f.Evaluar(xd);

            if (fxi * fxd > 0)
            {
                resultado.Mensaje = "Intervalo inválido: No hay cambio de signo (Teorema de Bolzano).";
                return resultado;
            }

            // ... (casos fxi == 0 o fxd == 0 igual que antes)

            double xr = 0, xrAnterior = 0, fxr = 0;
            double error = double.MaxValue;

            for (int i = 1; i <= iteraciones; i++)
            {
                // Pasamos fxi y fxd para no re-calcular dentro
                xr = CalcularXrOptimizado(xi, xd, fxi, fxd, metodo);

                fxr = f.Evaluar(xr); // Evaluamos fxr UNA sola vez por iteración

                if (i > 1)
                    error = Math.Abs((xr - xrAnterior) / xr);

                if (Math.Abs(fxr) < tolerancia || error < tolerancia)
                {
                    resultado.Raiz = xr;
                    resultado.Iteraciones = i;
                    resultado.Error = error;
                    resultado.Converge = true;
                    return resultado;
                }

                // Cambio de signo: solo actualizamos el límite que corresponde 
                // y su valor de función evaluada
                if (fxi * fxr > 0)
                {
                    xi = xr;
                    fxi = fxr; // Evitamos evaluar f(xi) en la próxima vuelta
                }
                else
                {
                    xd = xr;
                    fxd = fxr; // Evitamos evaluar f(xd) en la próxima vuelta
                }

                xrAnterior = xr;
            }

            resultado.Raiz = xr;
            resultado.Converge = false;
            resultado.Mensaje = "Límite de iteraciones alcanzado.";
            return resultado;
        }

        private double CalcularXrOptimizado(double xi, double xd, double fxi, double fxd, string metodo)
        {
            if (metodo.Equals("Biseccion", StringComparison.OrdinalIgnoreCase))
                return (xi + xd) / 2;

            // Control de división por cero para Regla Falsa
            if (Math.Abs(fxd - fxi) < 1e-15) return double.NaN;

            return (fxd * xi - fxi * xd) / (fxd - fxi);
        }
    }
}
