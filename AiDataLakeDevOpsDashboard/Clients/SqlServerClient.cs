// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class SqlServerClient
    {
        private string activeRequestCountQuery = @"SELECT count(*) FROM {0} WHERE status = 'Running'";

        public async Task<int> GetActiveQueryCount(string connectionString, string tableName)
        {
            int result = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var command = new SqlCommand(string.Format(activeRequestCountQuery, tableName), connection);

                using (var reader = await command.ExecuteReaderAsync())
                {

                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }                    
                };
            }

            return result;
        }
    }
}