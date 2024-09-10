//namespace Service.Api.Poliza.Model
//using System.Data; 
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
public class SqlServerDbService
{
    private readonly string _connectionString;

    public SqlServerDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<(DataTable, int, string)> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[] parameters)
    {
        DataTable dataTable = new DataTable();
        int resultCode = 0;
        string message = string.Empty;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                // Open the connection, execute the command, and fill the DataTable
                await connection.OpenAsync();
                adapter.Fill(dataTable);

                // Retrieve output parameters
                resultCode = (int)command.Parameters["@NumeroResultado"].Value;
                message = (string)command.Parameters["@Mensaje"].Value;
            }
        }

        return (dataTable, resultCode, message);
    }
}
