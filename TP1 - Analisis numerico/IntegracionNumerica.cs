using Calculus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class IntegracionNumerica
    {
        public double CalcularIntegralTrapeciosSimple(string funcion, double xi, double xd)
        {
            try
            {
                EvaluadorFuncion evaluador = new EvaluadorFuncion(funcion);
                // Fórmula: (f(a) + f(b)) * (b - a) / 2
                return ((evaluador.Evaluar(xi) + evaluador.Evaluar(xd)) * (xd - xi)) / 2;
            }
            catch
            {
                return double.NaN;
            }
        }

        public double CalcularIntegralTrapeciosMultiple(string funcion, double xi, double xd, int n)
        {
            try
            {
                EvaluadorFuncion evaluador = new EvaluadorFuncion(funcion);
                double h = (xd - xi) / n;
                double sum = 0;

                for (int i = 1; i < n; i++)
                {
                    sum += evaluador.Evaluar(xi + h * i);
                }

                return (h / 2) * (evaluador.Evaluar(xi) + 2 * sum + evaluador.Evaluar(xd));
            }
            catch
            {
                return double.NaN;
            }
        }

        public double CalcularIntegralSimpson1_3Simple(string funcion, double xi, double xd)
        {
            try
            {
                EvaluadorFuncion evaluador = new EvaluadorFuncion(funcion);
                double h = (xd - xi) / 2;
                return (h / 3) * (evaluador.Evaluar(xi) + 4 * evaluador.Evaluar(xi + h) + evaluador.Evaluar(xd));
            }
            catch
            {
                return double.NaN;
            }
        }

        public double CalcularIntegralSimpson3_8(string funcion, double xi, double xd)
        {
            try
            {
                EvaluadorFuncion evaluador = new EvaluadorFuncion(funcion);
                double h = (xd - xi) / 3;
                return (3 * h / 8) * (evaluador.Evaluar(xi) + 3 * evaluador.Evaluar(xi + h) + 3 * evaluador.Evaluar(xi + 2 * h) + evaluador.Evaluar(xd));
            }
            catch
            {
                return double.NaN;
            }
        }

        public double CalcularIntegralSimpsonMultipleCombinado(string funcion, double xi, double xd, int n)
        {
            try
            {
                EvaluadorFuncion evaluador = new EvaluadorFuncion(funcion);
                double h = (xd - xi) / n;
                double sumPares = 0, sumImpares = 0;
                double resultado = 0;
                bool simpson3_8Hecho = false;

                for (int i = 1; i < n; i++)
                {
                    // Si es impar y no hemos hecho Simpson 3/8 en los últimos 3 intervalos
                    if (n % 2 != 0 && !simpson3_8Hecho)
                    {
                        double nuevoXi = xi + h * (n - 3);

                        // Calculamos directamente con la fórmula para no instanciar otro evaluador
                        resultado = (3 * h / 8) * (evaluador.Evaluar(nuevoXi) + 3 * evaluador.Evaluar(nuevoXi + h) + 3 * evaluador.Evaluar(nuevoXi + 2 * h) + evaluador.Evaluar(xd));

                        n = n - 3;
                        xd = nuevoXi;
                        simpson3_8Hecho = true;
                    }

                    if (i % 2 == 0)
                    {
                        sumPares += evaluador.Evaluar(xi + h * i);
                    }
                    else
                    {
                        sumImpares += evaluador.Evaluar(xi + h * i);
                    }
                }

                // Sumamos el resultado del 1/3 Múltiple al que ya teníamos (si es que hicimos el 3/8)
                resultado += (h / 3) * (evaluador.Evaluar(xi) + 4 * sumImpares + 2 * sumPares + evaluador.Evaluar(xd));
                return resultado;
            }
            catch
            {
                return double.NaN;
            }
        }
    }
}
