using Microsoft.AspNetCore.Mvc;
using NetCoreYouTube.Models;
using NetCoreYouTube.Recursos;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

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
        //   using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-M91V76V;Initial Catalog=VisualGeneracionPdf;uid=sa;pwd=123456"))

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

                // Verifica si el token es válido (Número de Resultado = 2 según tu SP)
                if (numberResult == 0)
                {
                    // Parámetros para PKG_WEB_API.PDF_ENDOSO
                    var pdfParams = new[]
                    {
                    new SqlParameter("@poliza", data.poliza),
                    new SqlParameter("@cod_producto", data.cod_producto),
                    new SqlParameter("@tramite", data.tramite)
                };

                    // Ejecuta el procedimiento almacenado PKG_WEB_API.PDF_ENDOSO
                    DataTable validar1;
                    int numberResult1;
                    string varcharResult1;
                    using (var connection = new SqlConnection(_connectionString))
                    using (var command = new SqlCommand("PKG_WEB_API.PDF_ENDOSO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(pdfParams);

                        var adapter = new SqlDataAdapter(command);
                        validar1 = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(validar1);
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
                            // Continúa con el resto de tus operaciones

                            return Ok(new
                            {
                                error = false,
                                numbermsg = numberResult1,
                                msg = varcharResult1,
                                //data = base64Pdf  // Si generas un PDF, asegúrate de asignar `base64Pdf` aquí
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