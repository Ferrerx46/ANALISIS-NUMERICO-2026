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
            // Forzamos al hilo actual a usar punto decimal para evitar líos
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var json = e.WebMessageAsJson;

            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

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

            var respuesta = new
            {
                funcion = funcion,
                raiz = resultado.Raiz,
                iteraciones = resultado.Iteraciones,
                error = resultado.Error,
                mensaje = resultado.Mensaje, // IMPORTANTE
                xi = xi,
                xd = xd,
                metodo = metodo,
                converge = resultado.Converge
            };

            var opciones = new System.Text.Json.JsonSerializerOptions
            {
                // Esto permite que NaN e Infinity se serialicen sin romper todo
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            webView21.CoreWebView2.PostWebMessageAsJson(
                System.Text.Json.JsonSerializer.Serialize(respuesta, opciones)
            );
        }
    }
}
