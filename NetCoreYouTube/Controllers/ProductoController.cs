using Microsoft.AspNetCore.Mvc;
using NetCoreYouTube.Models;
using NetCoreYouTube.Recursos;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;
using System.Diagnostics.Metrics;

namespace NetCoreYouTube.Controllers
{
    [ApiController]
    [Route("producto")]
    public class ProductoController : ControllerBase
    {
        [HttpGet]
        [Route("listar")]
        public IActionResult ListarProductos()
        {
            try
            {
                // Parámetros para Categoria_Listar
                List<Parametro> parametrosCategoria = new List<Parametro>
         {
            new Parametro("@idEstado", "1")
          };

                // Llamada al procedimiento almacenado Categoria_Listar con el esquema dbo
                DataTable tCategoria = DBDatos.Listar("Categoria_Listar", parametrosCategoria);

                // Llamada al procedimiento almacenado Producto_Listar con el esquema dbo y sin parámetros
                DataTable tProducto = DBDatos.Listar("dbo.Producto_Listar", new List<Parametro>());

                // Convertir los DataTables a listas de diccionarios serializables
                var categoriaList = ConvertDataTableToList(tCategoria);
                var productoList = ConvertDataTableToList(tProducto);

                return Ok(new
                {
                    success = true,
                    message = "Éxito",
                    result = new
                    {
                        categoria = categoriaList,
                        producto = productoList,
                    }
                });
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error utilizando un logger si lo tienes configurado
                return StatusCode(500, new { success = false, message = "Error interno del servidor", details = ex.Message });
            }
        }

        // Función para convertir DataTable a una lista de diccionarios
        public static List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }

                list.Add(dict);
            }

            return list;
        }

    }

    [ApiController]
    [Route("api/[controller]")]
    public class EndosoController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=DESKTOP-M91V76V;Initial Catalog=VisualGeneracionPdf;User ID=sa;Password=123456";

        [HttpPost("GenerarPdfEndoso")]
        public async Task<IActionResult> GenerarPdf(PFDEndosoModel data)
        {
            try
            {
                // Log para verificar el token entrante
                Console.WriteLine($"Token: {data.token}");

                // Parámetros para VALIDAR_TOKEN
                var validationParams = new[]
                {
                new SqlParameter("@Token", data.token),
                new SqlParameter("@Mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output },
                new SqlParameter("@NumeroResultado", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

                // Ejecuta el procedimiento almacenado VALIDAR_TOKEN
                DataTable validar;
                int numberResult;
                string varcharResult;
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("VALIDAR_TOKEN", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(validationParams);

                    var adapter = new SqlDataAdapter(command);
                    validar = new DataTable();
                    await connection.OpenAsync();
                    adapter.Fill(validar);

                    numberResult = (int)command.Parameters["@NumeroResultado"].Value;
                    varcharResult = (string)command.Parameters["@Mensaje"].Value;
                }

                // Log para depuración
                Console.WriteLine($"Número de Resultado: {numberResult}");
                Console.WriteLine($"Mensaje: {varcharResult}");

                // Verifica si el token es válido (Número de Resultado = 0 según tu SP)
                if (numberResult == 0)
                {
                    // Parámetros para PDF_ENDOSO
                    var pdfParams = new[]
                    {
                    new SqlParameter("@poliza", data.poliza),
                    new SqlParameter("@cod_producto", data.cod_producto),
                    new SqlParameter("@tramite", data.tramite),
                    new SqlParameter("@NumeroResultado", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@Mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output }
                };

                    // Ejecuta el procedimiento almacenado PDF_ENDOSO
                    DataTable validar1;
                    int numberResult1;
                    string varcharResult1;
                    using (var connection = new SqlConnection(_connectionString))
                    using (var command = new SqlCommand("PDF_ENDOSO", connection))
                    { 
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(pdfParams);

                        var adapter = new SqlDataAdapter(command);
                        validar1 = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(validar1);

                        // Recupera los parámetros de salida
                        numberResult1 = (int)command.Parameters["@NumeroResultado"].Value;
                        varcharResult1 = (string)command.Parameters["@Mensaje"].Value;
                    }

                    if (numberResult1 == 0)
                    {
                        // Verifica si validar1 tiene filas antes de acceder a los datos
                        if (validar1 != null && validar1.Rows.Count > 0)
                        {
                            // Extrae la información para generar el PDF
                            var row = validar1.Rows[0];
                            string producto = row["producto"].ToString() ?? " ";
                            string poliza = row["poliza"].ToString() ?? " ";
                            string num_endoso = row["endoso"].ToString() ?? " ";
                            string fecha_vigencia = row["fec_inivigencia"].ToString() ?? " ";
                            string fecha_endoso = row["fecha_endoso"].ToString() ?? " ";
                            string nombre = row["nombre"].ToString() ?? " ";
                            string direccion = row["direccion"].ToString() ?? " ";
                            string tipo_doc = row["tipo_doc"].ToString() ?? " ";
                            string nro_iden = row["nro_iden"].ToString() ?? " ";
                            string telefono = row["telefono"].ToString() ?? " ";
                            string distrito = row["distrito"].ToString() ?? " ";
                            string provincia = row["provincia"].ToString() ?? " ";
                            string departamento = row["departamento"].ToString() ?? " ";
                            string moneda = row["moneda"].ToString() ?? " ";
                            string meses = row["MESES"].ToString() ?? " ";
                            string monto = row["monto"].ToString() ?? " ";
                            string descripcion = row["DESCRIPCION"].ToString() ?? " ";
                            string descripcionSubtitulo3 = row["DESCRIPCION_SUBTITULO3"].ToString() ?? " ";
                            string descripcionPago = row["DESCRIPCION_PAGO"].ToString() ?? " ";

                            // Continúa con el resto de tus operaciones (como generar un PDF)
                           string base64Pdf = CrearReporteLiquidacion(fecha_vigencia, fecha_endoso, poliza, producto, moneda, num_endoso, nombre, nro_iden, direccion, telefono, distrito, provincia, departamento, meses, descripcion, monto, tipo_doc, descripcionSubtitulo3, descripcionPago);

                            return Ok(new
                            {
                                error = false,
                                numbermsg = numberResult1,
                                msg = varcharResult1,
                                data = base64Pdf
                            });
                       
                    }
                        else
                        {
                            return BadRequest(new
                            {
                                error = true,
                                numbermsg = 1,
                                msg = "No se encontraron datos.",
                                data = 0
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            error = true,
                            numbermsg = numberResult1,
                            msg = varcharResult1,
                            data = 0
                        });
                    }
                }
                else
                {
                    // Token inválido
                    return BadRequest(new
                    {
                        error = true,
                        numbermsg = numberResult,
                        msg = varcharResult,
                        data = 0
                    });
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new
                {
                    error = true,
                    msg = "Error interno del servidor.",
                    data = 0
                });
            }
        }

        string CrearReporteLiquidacion(string fecha_vigencia, string fecha_endoso, string poliza, string producto, string moneda, string num_endoso, string nombre, string nro_iden, string direccion, string telefono, string distrito, string provincia, string departamento, string meses, string descripcion, string monto, string tipo_doc, string descripcionSubtitulo3, string descripcionPago)
        {
            // Crear documento PDF
            var document = new PdfDocument();
            document.Info.Title = "Liquidación de Devolución de Prima";

            // Crear una nueva página
            var page = document.AddPage();
            page.Size = PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            // Configurar fuentes
            var titleFont = new XFont("Tahoma", 14, XFontStyle.Bold);
            var normalFont = new XFont("Tahoma", 11, XFontStyle.Regular);
            var fechaFont = new XFont("Tahoma", 11, XFontStyle.Regular);
            var fechaboldFont = new XFont("Tahoma", 11, XFontStyle.Bold);
            var boldFont = new XFont("Tahoma", 10, XFontStyle.Bold);

            // Posición inicial
            int yPoint = 40;
            int xPoint = 70;

            XImage image = XImage.FromFile("C:\\WebApiUtilitarios\\Imagenes\\logoProtecta.png");
            gfx.DrawImage(image, -1, yPoint, 150, 35); //imagen, posicion x, posicion y , ancho, altura

            yPoint += 60;

            // Dibujar encabezado
            gfx.DrawString("Surquillo, " + fecha_endoso, fechaFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);

            yPoint += 40;

            // Dibujar título
            gfx.DrawString("LIQUIDACION DE DEVOLUCIÓN DE PRIMA", titleFont, XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopCenter);

            yPoint += 40;

            string text = "Protecta S.A. Compañía de Seguros y Reaseguros pone a su disposición, el pago correspondiente a la devolución de prima del producto " +
                      producto + ", el cual fue contratado con nuestra representada mediante póliza Nro. " + poliza + ".";

           // _utilitariosService.DrawJustifiedText(gfx, text, normalFont, new XRect(xPoint, yPoint, page.Width - 120, page.Height), 12);

            yPoint += 60;

            // Dibujar datos
            gfx.DrawString("Contratante", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Asegurado", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Póliza", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + poliza, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Producto", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Motivo Devolucion", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Importe", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Moneda", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + moneda, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 30;
            gfx.DrawString("Modalidad de Pago", boldFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(":   " + producto, boldFont, XBrushes.Black, new XRect(xPoint + 100, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 60;
            // Dibujar pie de página
            string text2 = "Por lo tanto, Protecta S.A. Compañía de Seguros y Reaseguros, ha cumplido en su totalidad con las obligaciones de pago referidas a la póliza " +
                           producto + " Nro. " + poliza + ".";
           // _utilitariosService.DrawJustifiedText(gfx, text2, normalFont, new XRect(xPoint, yPoint, page.Width - 120, page.Height), 12);

            yPoint += 60;
            gfx.DrawString("Agradecemos de antemano su atención de la presente.", normalFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 60;
            gfx.DrawString("Cordialmente,", normalFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width, page.Height), XStringFormats.TopLeft);

            yPoint += 40;

            // Dibujar firma
            XImage imagefirma = XImage.FromFile("C:\\WebApiUtilitarios\\Firmas\\FirmaOperaciones.jpg");
            gfx.DrawImage(imagefirma, 410, yPoint, 100, 50); //imagen, posicion x, posicion y , ancho, altura
            yPoint += 40;
            gfx.DrawString("__________________________", normalFont, XBrushes.Black, new XRect(xPoint, yPoint, page.Width - 120, page.Height), XStringFormats.TopRight);
            gfx.DrawString("Jefe de Operaciones", fechaboldFont, XBrushes.Black, new XRect(410, yPoint + 20, page.Width - 90, page.Height), XStringFormats.TopLeft);


            yPoint += 150;
            XImage imagepie = XImage.FromFile("C:\\WebApiUtilitarios\\Imagenes\\piepagina_cartaLiq.PNG");
            gfx.DrawImage(imagepie, xPoint, yPoint, 100, 10); //imagen, posicion x, posicion y , ancho, altura

            // Guardar el documento en un MemoryStream
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, false);

                byte[] pdfBytes = stream.ToArray();

                // Convertir el PDF a base 64
                string base64Pdf = Convert.ToBase64String(pdfBytes);
                return base64Pdf;

            }
        }

    }






    //public class EndosoController : ControllerBase
    //{
    //    private readonly SqlServerDbService _sqlServerDbService;

    //    public EndosoController(SqlServerDbService sqlServerDbService)
    //    {
    //        _sqlServerDbService = sqlServerDbService;
    //    }

    //    [HttpPost("GenerarPdfEndoso")]
    //    public async Task<IActionResult> GenerarPdf(PFDEndosoModel data)
    //    {
    //        try
    //        {
    //            // Log para verificar el token entrante
    //            Console.WriteLine($"Token: {data.token}");

    //            // Parámetros para VALIDAR_TOKEN
    //            var validationParams = new SqlParameter[]
    //            {
    //            new SqlParameter("@Token", data.token),
    //            new SqlParameter("@Mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output },
    //            new SqlParameter("@NumeroResultado", SqlDbType.Int) { Direction = ParameterDirection.Output }
    //            };

    //            // Ejecuta el procedimiento almacenado VALIDAR_TOKEN
    //            (DataTable validar, int numberResult, string varcharResult) = await _sqlServerDbService.ExecuteStoredProcedureAsync("VALIDAR_TOKEN", validationParams);

    //            // Log para depuración
    //            Console.WriteLine($"Número de Resultado: {numberResult}");
    //            Console.WriteLine($"Mensaje: {varcharResult}");

    //            // Verifica si el token es válido (Número de Resultado = 2 según tu SP)
    //            if (numberResult == 2)
    //            {
    //                // Parámetros para PKG_WEB_API.PDF_ENDOSO
    //                var pdfParams = new SqlParameter[]
    //                {
    //                new SqlParameter("@poliza", data.poliza),
    //                new SqlParameter("@cod_producto", data.cod_producto),
    //                new SqlParameter("@tramite", data.tramite)
    //                };

    //                // Ejecuta el procedimiento almacenado PKG_WEB_API.PDF_ENDOSO
    //                (DataTable validar1, int numberResult1, string varcharResult1) = await _sqlServerDbService.ExecuteStoredProcedureAsync("PKG_WEB_API.PDF_ENDOSO", pdfParams);

    //                if (numberResult1 == 0)
    //                {
    //                    // Verifica si validar1 tiene filas antes de acceder a los datos
    //                    if (validar1 != null && validar1.Rows.Count > 0)
    //                    {
    //                        // Extrae la información para generar el PDF
    //                        var row = validar1.Rows[0];
    //                        string producto = row["producto"].ToString() ?? " ";
    //                        string poliza = row["poliza"].ToString() ?? " ";
    //                        // Continúa con el resto de tus operaciones

    //                        return Ok(new
    //                        {
    //                            error = false,
    //                            numbermsg = numberResult1,
    //                            msg = varcharResult1,
    //                            //data = base64Pdf  // Si generas un PDF, asegúrate de asignar `base64Pdf` aquí
    //                        });
    //                    }
    //                    else
    //                    {
    //                        return BadRequest(new
    //                        {
    //                            error = true,
    //                            numbermsg = 1,
    //                            msg = "No se encontraron datos.",
    //                            data = 0
    //                        });
    //                    }
    //                }
    //                else
    //                {
    //                    return BadRequest(new
    //                    {
    //                        error = true,
    //                        numbermsg = numberResult1,
    //                        msg = varcharResult1,
    //                        data = 0
    //                    });
    //                }
    //            }
    //            else
    //            {
    //                // Token inválido
    //                return BadRequest(new
    //                {
    //                    error = true,
    //                    numbermsg = numberResult,
    //                    msg = varcharResult,
    //                    data = 0
    //                });
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Manejo de excepciones
    //            Console.WriteLine($"Error: {ex.Message}");
    //            return StatusCode(500, new
    //            {
    //                error = true,
    //                msg = "Error interno del servidor.",
    //                data = 0
    //            });
    //        }
    //    }
    //}




}