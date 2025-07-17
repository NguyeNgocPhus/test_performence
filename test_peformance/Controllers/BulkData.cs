using System.Data;
using Microsoft.Data.SqlClient;

namespace test_peformance.Controllers;

public class BulkData
{
    public async Task CreateTable(DataTable dt, string  tableName)
    {
        using (var destinationConnection = new SqlConnection("Data Source=localhost;Initial Catalog=master;User id=sa;Password=YourStrong!Passw0rd; TrustServerCertificate=True"))
        {
            using (var bulkCopy = new SqlBulkCopy(destinationConnection))
            {
                int backSize = 10000;
                int timeOut = 120;
                await destinationConnection.OpenAsync();
                if(timeOut > 0)
                {
                    bulkCopy.BulkCopyTimeout = timeOut * (dt.Rows.Count / backSize);
                }
                bulkCopy.BatchSize = backSize;
                bulkCopy.DestinationTableName = tableName;
                await bulkCopy.WriteToServerAsync(dt);
                await destinationConnection.CloseAsync();
            }
        }
    }
}