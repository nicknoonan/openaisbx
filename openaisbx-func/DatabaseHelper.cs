using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace openaisbx
{
    public class DatabaseHelper
    {
        public static async Task<int> ExecuteNonQuery(ILogger log, string query, Dictionary<string, string> query_params)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")))
                {
                    connection.Open();
                    log.LogInformation("DB connection opened");
                    SqlCommand command = new SqlCommand(query, connection);
                    if (query_params != null)
                    {
                        foreach (var q_param in query_params)
                        {
                            command.Parameters.AddWithValue(q_param.Key, q_param.Value);
                        }
                    }
                    int command_return = await command.ExecuteNonQueryAsync();
                    log.LogInformation("DB connection closing...");
                    return command_return;
                }
            }
            catch (Exception ex) {
                log.LogError(ex.Message);
                return 0;
            }
        }
    }
}
