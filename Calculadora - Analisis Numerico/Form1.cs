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
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            var json = e.WebMessageAsJson;
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            string tipoAccion =
                root.TryGetProperty("tipoAccion", out var t)
                ? t.GetString()
                : "RAICES";

            var opcionesSer = new System.Text.Json.JsonSerializerOptions
            {
                NumberHandling =
                    System.Text.Json.Serialization.JsonNumberHandling
                    .AllowNamedFloatingPointLiterals
            };

            // =====================================================
            // UNIDAD 2 - SISTEMAS
            // =====================================================
            if (tipoAccion == "SISTEMA")
            {
                int dim = root.GetProperty("dimension").GetInt32();
                string metodo = root.GetProperty("metodo").GetString();

                int iteracionesMax =
                    root.TryGetProperty("iteraciones", out var iProp)
                    ? iProp.GetInt32()
                    : 100;

                double tolerancia =
                    root.TryGetProperty("tolerancia", out var tProp)
                    ? tProp.GetDouble()
                    : 0.0001;

                double[,] matrizCsharp = new double[dim, dim + 1];
                var matrizJson = root.GetProperty("matriz");

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j <= dim; j++)
                    {
                        matrizCsharp[i, j] =
                            matrizJson[i][j].GetDouble();
                    }
                }

                var mSistemas = new SistemasDeEcuaciones();
                ResultadoSistema resultadoSist;

                if (metodo == "GaussJordan")
                {
                    resultadoSist =
                        mSistemas.ResolverGaussJordan(
                            matrizCsharp,
                            dim
                        );
                }
                else
                {
                    resultadoSist =
                        mSistemas.ResolverGaussSeidel(
                            matrizCsharp,
                            dim,
                            tolerancia
                        );
                }

                var respuestaSistema = new
                {
                    tipoAccion = "SISTEMA",
                    metodo = resultadoSist.Metodo,
                    soluciones = resultadoSist.Soluciones,
                    iteraciones = resultadoSist.Iteraciones,
                    error = resultadoSist.Error,
                    converge = resultadoSist.Converge,
                    mensaje = resultadoSist.Mensaje
                };

                webView21.CoreWebView2.PostWebMessageAsJson(
                    System.Text.Json.JsonSerializer.Serialize(
                        respuestaSistema,
                        opcionesSer
                    )
                );
            }

            // =====================================================
            // UNIDAD 3 - REGRESIONES
            // =====================================================
            else if (tipoAccion == "REGRESION")
            {
                string metodo =
                    root.GetProperty("metodo").GetString();

                int grado =
                    root.GetProperty("grado").GetInt32();

                double tolerancia =
                    root.GetProperty("tolerancia").GetDouble();

                var puntosJson =
                    root.GetProperty("puntos");

                List<double[]> puntos =
                    new List<double[]>();

                foreach (var punto in puntosJson.EnumerateArray())
                {
                    puntos.Add(new double[]
                    {
                punto[0].GetDouble(),
                punto[1].GetDouble()
                    });
                }

                Regresiones reg = new Regresiones();

                ResultadoRegresion resultado;

                if (metodo == "Lineal")
                {
                    resultado =
                        reg.CalcularRegresionLineal(
                            puntos,
                            tolerancia
                        );
                }
                else
                {
                    resultado =
                        reg.CalcularRegresionPolinomial(
                            grado,
                            puntos,
                            tolerancia
                        );
                }

                var respuestaRegresion = new
                {
                    tipoAccion = "REGRESION",
                    funcion = resultado.Funcion,
                    r = resultado.R,
                    efectividad = resultado.Efectividad
                };

                webView21.CoreWebView2.PostWebMessageAsJson(
                    System.Text.Json.JsonSerializer.Serialize(
                        respuestaRegresion,
                        opcionesSer
                    )
                );
            }

            // =====================================================
            // UNIDAD 1 - RAICES
            // =====================================================
            else
            {
                string funcion =
                    root.GetProperty("funcion").GetString();

                double xi =
                    root.GetProperty("xi").GetDouble();

                double xd =
                    root.GetProperty("xd").GetDouble();

                int iteraciones =
                    root.GetProperty("iteraciones").GetInt32();

                double tolerancia =
                    root.GetProperty("tolerancia").GetDouble();

                string metodo =
                    root.GetProperty("metodo").GetString();

                ResultadoMetodo resultado;

                if (metodo == "Biseccion" ||
                    metodo == "ReglaFalsa")
                {
                    var m = new MetodosCerrados();

                    resultado = m.Resolver(
                        funcion,
                        xi,
                        xd,
                        tolerancia,
                        iteraciones,
                        metodo
                    );
                }
                else
                {
                    var m = new MetodosAbiertos();

                    resultado = m.Resolver(
                        funcion,
                        xi,
                        xd,
                        tolerancia,
                        iteraciones,
                        metodo
                    );
                }

                var respuestaRaices = new
                {
                    tipoAccion = "RAICES",
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
                    System.Text.Json.JsonSerializer.Serialize(
                        respuestaRaices,
                        opcionesSer
                    )
                );
            }
        }
    }
}
