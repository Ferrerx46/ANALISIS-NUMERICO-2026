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

                // Si n es par, hacemos Simpson 1/3 múltiple normal
                if (n % 2 == 0)
                {
                    double sumaPares = 0;
                    double sumaImpares = 0;

                    for (int i = 1; i < n; i++)
                    {
                        double fx = evaluador.Evaluar(xi + i * h);

                        if (i % 2 == 0)
                            sumaPares += fx;
                        else
                            sumaImpares += fx;
                    }

                    return (h / 3) *
                           (
                               evaluador.Evaluar(xi)
                               + 4 * sumaImpares
                               + 2 * sumaPares
                               + evaluador.Evaluar(xd)
                           );
                }

                // ---------------------------
                // n impar: combinar 1/3 y 3/8
                // ---------------------------

                int n13 = n - 3;

                // Simpson 1/3 en los primeros n-3 intervalos
                double xd13 = xi + n13 * h;

                double sumaPares13 = 0;
                double sumaImpares13 = 0;

                for (int i = 1; i < n13; i++)
                {
                    double fx = evaluador.Evaluar(xi + i * h);

                    if (i % 2 == 0)
                        sumaPares13 += fx;
                    else
                        sumaImpares13 += fx;
                }

                double resultado13 =
                    (h / 3) *
                    (
                        evaluador.Evaluar(xi)
                        + 4 * sumaImpares13
                        + 2 * sumaPares13
                        + evaluador.Evaluar(xd13)
                    );

                // Simpson 3/8 en los últimos 3 intervalos
                double xi38 = xd13;

                double resultado38 =
                    (3 * h / 8) *
                    (
                        evaluador.Evaluar(xi38)
                        + 3 * evaluador.Evaluar(xi38 + h)
                        + 3 * evaluador.Evaluar(xi38 + 2 * h)
                        + evaluador.Evaluar(xd)
                    );

                return resultado13 + resultado38;
            }
            catch
            {
                return double.NaN;
            }
        }
    }
}
