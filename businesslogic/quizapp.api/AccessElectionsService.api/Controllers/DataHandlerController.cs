using AccessElectionsService.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace AccessElectionsService.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataHandlerController : Controller
    {
        private readonly IConfiguration _configuration;
        public DataHandlerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("load-data")]
        public IActionResult LoadData([FromBody] DataHandler dataHandler)
        {
            try
            {
                RestoreDbFromBackUp(dataHandler.DBBackupFilePath);
                CopyDataToTarget();
                RemoveTheTempDb();
                return Ok("Data copy process completed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private void RemoveTheTempDb()
        {
            var databaseName = "CompanyDB";
            // Set the connection string for the master database
            string? masterConnectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Create SqlConnection for the master database
            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                // Set the database to SINGLE_USER mode
                using (SqlCommand setSingleUserCommand = masterConnection.CreateCommand())
                {
                    setSingleUserCommand.CommandText = $"USE master; ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    setSingleUserCommand.ExecuteNonQuery();
                }

                // Drop the database
                using (SqlCommand dropCommand = masterConnection.CreateCommand())
                {
                    dropCommand.CommandText = $"USE master; DROP DATABASE [{databaseName}];";
                    dropCommand.ExecuteNonQuery();

                    Console.WriteLine($"Database '{databaseName}' dropped successfully.");
                }
            }
        }

        private void CopyDataToTarget()
        {
            // Set the connection string for the source database (restored database)
            string? sourceConnectionString = _configuration.GetConnectionString("DataSourceConnection");

            // Set the connection string for the target database
            string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

            // Create SqlConnection for the source database
            // Create SqlConnection for the source database
            using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();

                // Get column names from the Employee table in the source database
                DataTable schemaTable = sourceConnection.GetSchema("Columns", new[] { null, null, "Employee", null });
                string?[] columnNames = schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray();

                // Create a SqlCommand to select data from the Employee table
                using (SqlCommand selectCommand = sourceConnection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT * FROM Employee";

                    // Create a SqlDataReader to read the data
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        // Create SqlConnection for the target database
                        using (SqlConnection targetConnection = new SqlConnection(targetConnectionString))
                        {
                            targetConnection.Open();

                            // Create a SqlCommand to insert data into the Employee table in the target database
                            using (SqlCommand insertCommand = targetConnection.CreateCommand())
                            {
                                // Prepare the insert command dynamically based on column names
                                insertCommand.CommandText = $"INSERT INTO Employee ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select(col => $"@{col}"))})";

                                // Define parameters for the insert command
                                foreach (var columnName in columnNames)
                                {
                                    insertCommand.Parameters.Add($"@{columnName}", SqlDbType.NVarChar, 50); // Adjust the SqlDbType accordingly
                                }

                                // Iterate through the source data and insert into the target database
                                while (reader.Read())
                                {
                                    // Set parameter values dynamically
                                    foreach (var columnName in columnNames)
                                    {
                                        insertCommand.Parameters[$"@{columnName}"].Value = reader[columnName];
                                    }

                                    try
                                    {
                                        // Attempt to execute the insert command
                                        insertCommand.ExecuteNonQuery();
                                        Console.WriteLine($"Data inserted for EmployeeID: {reader["EmployeeID"]}");
                                    }
                                    catch (SqlException ex)
                                    {
                                        // Handle the exception for duplicate records
                                        if (ex.Number == 2627 || ex.Number == 2601)
                                        {
                                            // SQL Server error numbers for unique constraint violation
                                            Console.WriteLine($"Skipped duplicate record for EmployeeID: {reader["EmployeeID"]}");
                                        }
                                        else
                                        {
                                            // Re-throw the exception if it's not a duplicate key violation
                                            throw;
                                        }
                                    }
                                }
                                Console.WriteLine("Data copy process completed.");
                            }
                        }
                    }
                    sourceConnection.Close();
                }
            }
        }

        private void RestoreDbFromBackUp(string dbBackupFilePath)
        {
            // Set the connection string for the master database
            string? connectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Provide the path to your backup file
            string backupFilePath = dbBackupFilePath;

            // Specify the database name you want to restore
            string databaseName = "CompanyDB";

            // Create a SqlConnection to the master database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlCommand to execute the restore command
                using (SqlCommand command = connection.CreateCommand())
                {
                    // Specify the restore command with the backup file path and target database name
                    command.CommandText = $"RESTORE DATABASE [{databaseName}] FROM DISK = '{backupFilePath}' WITH REPLACE, STATS = 10";

                    // Execute the restore command
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Database '{databaseName}' restored successfully.");
                connection.Close();
            }
        }
    }
}
