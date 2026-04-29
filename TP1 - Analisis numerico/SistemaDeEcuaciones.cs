using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP1___Analisis_numerico
{
    using System;

    public class SistemasDeEcuaciones
    {
        // MÉTODO: GAUSS-JORDAN
        public ResultadoSistema ResolverGaussJordan(double[,] matrizOriginal, int dimension)
        {
            double[,] matriz = (double[,])matrizOriginal.Clone();
            var resultado = new ResultadoSistema
            {
                Metodo = "Gauss-Jordan",
                Iteraciones = 1 // Es un método directo, siempre es 1
            };

            try
            {
                for (int rowDiag = 0; rowDiag < dimension; rowDiag++)
                {
                    double coeficienteDiagonal = matriz[rowDiag, rowDiag];

                    if (Math.Abs(coeficienteDiagonal) < 1e-12)
                    {
                        resultado.Converge = false;
                        resultado.Mensaje = "Error: Cero en la diagonal principal. Se requiere pivoteo.";
                        return resultado;
                    }

                    // Hacer 1 el pivote
                    for (int col = 0; col < dimension + 1; col++)
                        matriz[rowDiag, col] /= coeficienteDiagonal;

                    // Hacer 0 el resto de la columna
                    for (int row = 0; row < dimension; row++)
                    {
                        if (rowDiag != row)
                        {
                            double coeficienteCero = matriz[row, rowDiag];
                            for (int col = 0; col < dimension + 1; col++)
                                matriz[row, col] -= coeficienteCero * matriz[rowDiag, col];
                        }
                    }
                }

                double[] soluciones = new double[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    soluciones[i] = matriz[i, dimension];
                }

                resultado.Soluciones = soluciones;
                resultado.Converge = true;
                resultado.Mensaje = "Éxito.";
            }
            catch (Exception ex)
            {
                resultado.Converge = false;
                resultado.Mensaje = "Error crítico: " + ex.Message;
            }

            return resultado;
        }

        // MÉTODO: GAUSS-SEIDEL
        public ResultadoSistema ResolverGaussSeidel(double[,] matriz, int dimension, double tolerancia = 0.0001)
        {
            var resultado = new ResultadoSistema { Metodo = "Gauss-Seidel" };
            double[] vectorRes = new double[dimension];
            double[] vectorAnt = new double[dimension];
            int contador = 0;
            bool esSolucion = false;

            while (contador < 100 && !esSolucion)
            {
                contador++;
                Array.Copy(vectorRes, vectorAnt, dimension);

                for (int row = 0; row < dimension; row++)
                {
                    double suma = matriz[row, dimension];
                    for (int col = 0; col < dimension; col++)
                    {
                        if (row != col)
                            suma -= matriz[row, col] * vectorRes[col];
                    }
                    vectorRes[row] = suma / matriz[row, row];
                }

                int cumplenTolerancia = 0;
                double errorMaximo = 0;

                for (int i = 0; i < dimension; i++)
                {
                    double error = (vectorRes[i] != 0)
                        ? Math.Abs((vectorRes[i] - vectorAnt[i]) / vectorRes[i])
                        : Math.Abs(vectorRes[i] - vectorAnt[i]);

                    if (error < tolerancia) cumplenTolerancia++;
                    if (error > errorMaximo) errorMaximo = error;
                }

                esSolucion = (cumplenTolerancia == dimension);
                resultado.Error = errorMaximo;
            }

            resultado.Soluciones = vectorRes;
            resultado.Iteraciones = contador;
            resultado.Converge = esSolucion;
            resultado.Mensaje = esSolucion ? "Convergió correctamente." : "Superó el límite de iteraciones.";

            return resultado;
        }
    }
}
