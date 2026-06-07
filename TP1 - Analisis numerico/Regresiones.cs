using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    public class Regresiones
    {
        public ResultadoRegresion CalcularRegresionLineal(List<double[]> PuntosCargados, double tolerancia)
        {
            int n = PuntosCargados.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            // Pasos 2 al 5: Sumatorias
            foreach (var p in PuntosCargados)
            {
                sumX += p[0];
                sumY += p[1];
                sumXY += p[0] * p[1];
                sumX2 += Math.Pow(p[0], 2);
            }

            // Pasos 6 y 7: Calcular a1 y a0
            double a1 = (n * sumXY - sumX * sumY) / (n * sumX2 - Math.Pow(sumX, 2));
            double a0 = (sumY / n) - a1 * (sumX / n);

            // Paso 8: Calcular St y Sr
            double st = 0, sr = 0;
            double mediaY = sumY / n;

            foreach (var p in PuntosCargados)
            {
                st += Math.Pow(mediaY - p[1], 2);
                sr += Math.Pow((a1 * p[0] + a0) - p[1], 2);
            }

            // Paso 9: Coeficiente de correlación
            double r = Math.Sqrt((st - sr) / st) * 100;

            // Paso 10: Resultados
            string efectividad = (r >= tolerancia * 100) ? "El ajuste es aceptable" : "El ajuste no es aceptable";
            string funcion = $"y = {Math.Round(a1, 4)}x + {Math.Round(a0, 4)}";

            return new ResultadoRegresion
            {
                Funcion = funcion,
                R = r,
                Efectividad = efectividad
            };
        }

        public double[,] GenerarMatrizPolinomial(int grado, List<double[]> PuntosCargados)
        {
            int dimension = grado + 1;
            double[,] matriz = new double[dimension, dimension + 1];

            foreach (var punto in PuntosCargados)
            {
                double x = punto[0];
                double y = punto[1];

                for (int fila = 0; fila < dimension; fila++)
                {
                    for (int col = 0; col < dimension; col++)
                    {
                        // Calcula los coeficientes de las incógnitas 
                        matriz[fila, col] += Math.Pow(x, fila + col);
                    }
                    // Calcula los términos independientes 
                    matriz[fila, dimension] += y * Math.Pow(x, fila);
                }
            }
            return matriz;
        }

        public ResultadoRegresion CalcularRegresionPolinomial(int grado, List<double[]> PuntosCargados, double tolerancia)
        {
            // 1. Generar la matriz polinomial según el grado
            double[,] matriz = GenerarMatrizPolinomial(grado, PuntosCargados);
            int dimension = grado + 1;

            // 2. Resolver utilizando tu método de Gauss-Jordan 
            SistemasDeEcuaciones sisEq = new SistemasDeEcuaciones();
            ResultadoSistema resultadoGJ = sisEq.ResolverGaussJordan(matriz, dimension);

            // Validación por si la matriz no converge
            if (!resultadoGJ.Converge)
            {
                return new ResultadoRegresion
                {
                    Funcion = "No se pudo resolver la matriz (Error Gauss-Jordan)",
                    R = 0,
                    Efectividad = "Fallo"
                };
            }

            double[] vectorResultado = resultadoGJ.Soluciones;

            // 3. Armar la función iterando el vectorResultado 
            string funcion = string.Empty;
            string signo = string.Empty;

            for (int i = 0; i < vectorResultado.Length; i++)
            {
                double ai = Math.Round(vectorResultado[i], 4);

                if (i == 0)
                {
                    funcion = $"{ai}";
                }
                else if (i == 1)
                {
                    funcion = $"{ai}x {signo} {funcion}";
                }
                else
                {
                    funcion = $"{ai}x^{i} {signo} {funcion}";
                }

                signo = (ai >= 0) ? "+" : "";
            }

            // 4. Calcular el coeficiente de correlación (r) 
            double sumY = 0;
            int n = PuntosCargados.Count;
            foreach (var p in PuntosCargados) sumY += p[1];
            double mediaY = sumY / n;

            double sr = 0, st = 0;

            foreach (var punto in PuntosCargados)
            {
                double x = punto[0];
                double y = punto[1];
                double suma = 0;

                for (int i = 0; i < vectorResultado.Length; i++)
                {
                    suma += vectorResultado[i] * Math.Pow(x, i);
                }

                // Cálculo de sumatorias de errores
                sr += Math.Pow(y - suma, 2);
                st += Math.Pow(mediaY - y, 2);
            }

            double r = Math.Sqrt((st - sr) / st) * 100;

            // 5. Determinar efectividad
            string efectividad = (r >= tolerancia * 100) ? "El ajuste es aceptable" : "El ajuste no es aceptable";

            return new ResultadoRegresion
            {
                Funcion = "y = " + funcion.Trim(),
                R = r,
                Efectividad = efectividad
            };
        }
    }
}
