using System.Data;
using System.Data.SqlClient;

namespace NetCoreYouTube.Recursos
{
    public class DBDatos
    {
       

        public static object ListarTablas(string nombreProcedimiento, List<Parametro> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-M91V76V;Initial Catalog=VisualGeneracionPdf;User ID=sa;Password=123456"))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(nombreProcedimiento, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                cmd.Parameters.AddWithValue(parametro.Nombre, parametro.Valor);
                            }
                        }

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);

                        // Convierte el primer DataTable del DataSet en una lista serializable
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            return ConvertDataTableToList(dt);  // Convierte DataTable a un formato serializable
                        }

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Maneja el error
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public static DataTable Listar(string nombreProcedimiento, List<Parametro> parametros = null)
        {
            // Creamos el DataTable donde se guardarán los resultados
            DataTable tabla = new DataTable();

            try
            {
                // Abrimos la conexión con la base de datos utilizando un bloque "using"
                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-M91V76V;Initial Catalog=VisualGeneracionPdf;User ID=sa;Password=123456"))

                {
                    // Configuramos el comando para ejecutar el procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand(nombreProcedimiento, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Si existen parámetros, los añadimos al comando
                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                cmd.Parameters.AddWithValue(parametro.Nombre, parametro.Valor);
                            }
                        }

                        // Abrimos la conexión a la base de datos
                        con.Open();

                        // Usamos SqlDataAdapter para ejecutar el comando y llenar el DataTable
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(tabla);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejar errores específicos de SQL Server, puedes cambiarlo por un log
                Console.WriteLine($"Error de SQL: {sqlEx.Message}");
                throw; // Volvemos a lanzar la excepción para que sea manejada más adelante
            }
            catch (Exception ex)
            {
                // Manejo general de excepciones, puedes cambiarlo por un log
                Console.WriteLine($"Error general: {ex.Message}");
                throw; // Volvemos a lanzar la excepción
            }

            // Retornamos el DataTable con los datos obtenidos
            return tabla;
        }


        
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


}
