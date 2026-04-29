using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using TP1___Analisis_numerico;

namespace Calculadora___Analisis_Numerico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InicializarWebView();
        }

        private async void InicializarWebView()
        {
            await webView21.EnsureCoreWebView2Async(null);

            webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
            webView21.CoreWebView2.WebMessageReceived += WebMessageReceived;

            string ruta = Path.Combine(Application.StartupPath, "index.html");
            webView21.Source = new Uri($"file:///{ruta.Replace("\\", "/")}");
        }

        private void WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var json = e.WebMessageAsJson;
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            // 1. Detectar qué tipo de operación es
            // Usamos GetProperty y TryGetProperty para no romper si falta alguna
            string tipoAccion = root.TryGetProperty("tipoAccion", out var t) ? t.GetString() : "RAICES";

            var opcionesSer = new System.Text.Json.JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            if (tipoAccion == "SISTEMA")
            {
                // --- LÓGICA UNIDAD 2: SISTEMAS ---
                int dim = root.GetProperty("dimension").GetInt32();
                string metodo = root.GetProperty("metodo").GetString();

                // Parsear la matriz desde el JSON
                double[,] matrizCsharp = new double[dim, dim + 1];
                var matrizJson = root.GetProperty("matriz");

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j <= dim; j++)
                    {
                        matrizCsharp[i, j] = matrizJson[i][j].GetDouble();
                    }
                }

                var mSistemas = new SistemasDeEcuaciones();
                ResultadoSistema resultadoSist;

                if (metodo == "GaussJordan")
                {
                    resultadoSist = mSistemas.ResolverGaussJordan(matrizCsharp, dim);
                }
                else // Gauss-Seidel
                {
                    // Podrías sacar la tolerancia del JSON si la agregas al front
                    resultadoSist = mSistemas.ResolverGaussSeidel(matrizCsharp, dim, 0.0001);
                }

                // Enviamos la respuesta de vuelta
                webView21.CoreWebView2.PostWebMessageAsJson(
                    System.Text.Json.JsonSerializer.Serialize(resultadoSist, opcionesSer)
                );
            }
            else
            {
                // --- LÓGICA UNIDAD 1: RAÍCES (Tu código original) ---
                string funcion = root.GetProperty("funcion").GetString();
                double xi = root.GetProperty("xi").GetDouble();
                double xd = root.GetProperty("xd").GetDouble();
                int iteraciones = root.GetProperty("iteraciones").GetInt32();
                double tolerancia = root.GetProperty("tolerancia").GetDouble();
                string metodo = root.GetProperty("metodo").GetString();

                ResultadoMetodo resultado;
                if (metodo == "Biseccion" || metodo == "ReglaFalsa")
                {
                    var m = new MetodosCerrados();
                    resultado = m.Resolver(funcion, xi, xd, tolerancia, iteraciones, metodo);
                }
                else
                {
                    var m = new MetodosAbiertos();
                    resultado = m.Resolver(funcion, xi, xd, tolerancia, iteraciones, metodo);
                }

                var respuestaRaices = new
                {
                    funcion = funcion,
                    raiz = resultado.Raiz,
                    iteraciones = resultado.Iteraciones,
                    error = resultado.Error,
                    mensaje = resultado.Mensaje,
                    xi = xi,
                    xd = xd,
                    metodo = metodo,
                    converge = resultado.Converge
                };

                webView21.CoreWebView2.PostWebMessageAsJson(
                    System.Text.Json.JsonSerializer.Serialize(respuestaRaices, opcionesSer)
                );
            }
        }
    }
}
