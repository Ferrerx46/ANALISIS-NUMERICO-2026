using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class MetodosAbiertos
    {
        public ResultadoMetodo Resolver(string funcionStr, double xi, double xd, double tolerancia, int iteraciones, string metodo)
        {
            var f = new EvaluadorFuncion(funcionStr);
            var resultado = new ResultadoMetodo { Metodo = metodo, Funcion = funcionStr };

            double xr = 0, xrAnterior = 0;
            double errorRelativo = 100;

            for (int i = 1; i <= iteraciones; i++)
            {
                xr = CalcularXr(f, xi, xd, metodo);
                resultado.Raiz = xr;

                if (double.IsNaN(xr) || double.IsInfinity(xr))
                {
                    resultado.Converge = false;
                    resultado.Mensaje = "El método divergió (división por cero o pendiente nula).";
                    resultado.Iteraciones = i;
                    return resultado;
                }

                if (i > 1)
                {
                    errorRelativo = Math.Abs(xr - xrAnterior);

                    // 🔹 criterio extra
                    if (errorRelativo < tolerancia)
                    {
                        resultado.Raiz = xr;
                        resultado.Iteraciones = i;
                        resultado.Error = errorRelativo;
                        resultado.Converge = true;
                        return resultado;
                    }
                }

                // 🔹 criterio por valor de función
                if (Math.Abs(f.Evaluar(xr)) < tolerancia)
                {
                    resultado.Raiz = xr;
                    resultado.Iteraciones = i;
                    resultado.Error = errorRelativo;
                    resultado.Converge = true;
                    return resultado;
                }

                // 🔹 actualización
                if (metodo == "Tangente")
                {
                    xi = xr;
                }
                else // Secante
                {
                    xi = xd;
                    xd = xr;
                }

                xrAnterior = xr;
            }

            resultado.Raiz = xr;
            resultado.Iteraciones = iteraciones;
            resultado.Error = errorRelativo;
            resultado.Converge = false;
            resultado.Mensaje = "Máximo de iteraciones alcanzado.";
            return resultado;
        }

        private double CalcularXr(EvaluadorFuncion f, double xi, double xd, string metodo)
        {
            if (metodo == "Tangente")
            {
                double fxi = f.Evaluar(xi);
                double derivada = f.Derivada(xi);

                if (Math.Abs(derivada) < 1e-8)
                    return double.NaN;

                return xi - (fxi / derivada);
            }
            else // Secante
            {
                double fxi = f.Evaluar(xi);
                double fxd = f.Evaluar(xd);
                double denominador = fxd - fxi;

                if (Math.Abs(denominador) < 1e-12)
                    return double.NaN;

                return xd - (fxd * (xd - xi)) / denominador;
            }
        }
    }
}
