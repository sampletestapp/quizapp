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
        private string[]? _restoreTableNames;
        private string? _restoreDbName;
        public DataHandlerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("load-data")]
        public IActionResult LoadData([FromBody] DataHandler dataHandler)
        {
            try
            {
                dataHandler.DBBackupFilePath = "D:\\Work\\Tobedeeleted\\Naren\\T011.bak";
                GetRestoreTableDbNames();
                RestoreDbFromBackUp(dataHandler.DBBackupFilePath);
                CopyDataToTarget();
                //RemoveTheTempDb();
                return Ok("Data copy process completed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private void GetRestoreTableDbNames()
        {
            // Get the TablesToRestore section
            var tablesSection = _configuration.GetSection("RestoreInfo");

            // Get the comma-separated table names
            var tableNames = tablesSection.GetValue<string>("Tables");
            var dbName = tablesSection.GetValue<string>("Db");

            // Split the table names into an array
            _restoreTableNames = tableNames?.Split(',').Select(tableName => tableName.Trim()).ToArray();
            _restoreDbName = dbName?.ToString();
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

            // List of tables to copy data from
            string[] tablesToCopy = { "SurveyResponse", "PollingPlacePhoto", "ActivityLog", "SurveyResponseHistory" };

            foreach (var tableName in tablesToCopy)
            {
                // Create SqlConnection for the source database
                using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
                {
                    sourceConnection.Open();

                    // Get column names from the current table in the source database
                    DataTable schemaTable = sourceConnection.GetSchema("Columns", new[] { null, null, tableName, null });
                    string?[] columnNames = schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray();

                    // Create a SqlCommand to select data from the current table
                    using (SqlCommand selectCommand = sourceConnection.CreateCommand())
                    {
                        selectCommand.CommandText = $"SELECT * FROM {tableName}";

                        // Create a SqlDataReader to read the data
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            // Create SqlConnection for the target database
                            using (SqlConnection targetConnection = new SqlConnection(targetConnectionString))
                            {
                                targetConnection.Open();

                                // Create a SqlCommand to insert data into the current table in the target database
                                using (SqlCommand insertCommand = targetConnection.CreateCommand())
                                {
                                    // Prepare the insert command dynamically based on column names
                                    insertCommand.CommandText = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select(col => $"@{col}"))})";

                                    // Define parameters for the insert command
                                    foreach (var columnName in columnNames)
                                    {
                                        //if (columnName == "Response") // Assuming "Response" is your XML column name
                                        //{
                                        //    insertCommand.Parameters.Add($"@{columnName}", SqlDbType.Xml);
                                        //}
                                        //else
                                        //{
                                        if (columnName == "PollingPlacePhoto")
                                        {
                                            insertCommand.Parameters["PollingPlacePhoto"].Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            // Adjust SqlDbType accordingly based on the data type of the column
                                            SqlDbType sqlDbType = GetSqlDbTypeForColumn(columnName, schemaTable);
                                            insertCommand.Parameters.Add($"@{columnName}", sqlDbType, GetSizeForColumn(columnName, schemaTable));
                                        }
                                        //}
                                    }

                                    // Iterate through the source data and insert into the target database
                                    while (reader.Read())
                                    {
                                        // Set parameter values dynamically
                                        for (int i = 0; i < columnNames.Length; i++)
                                        {
                                            var columnName = columnNames[i];
                                            var columnValue = reader[columnName];

                                            // Check for DBNull and handle accordingly
                                            insertCommand.Parameters[$"@{columnName}"].Value = columnValue == DBNull.Value ? DBNull.Value : columnValue;
                                        }

                                        try
                                        {
                                            // Attempt to execute the insert command
                                            insertCommand.ExecuteNonQuery();
                                            //Console.WriteLine($"Data inserted for {tableName} with Primary Key: {reader["PollingPlacePhotoUID"]}");
                                        }
                                        catch (SqlException ex)
                                        {
                                            // Handle the exception for duplicate records
                                            if (ex.Number == 2627 || ex.Number == 2601)
                                            {
                                                // SQL Server error numbers for unique constraint violation
                                                //Console.WriteLine($"Skipped duplicate record for {tableName} with Primary Key: {reader["PollingPlacePhotoUID"]}");
                                            }
                                            else
                                            {
                                                // Re-throw the exception if it's not a duplicate key violation
                                                throw;
                                            }
                                        }
                                    }
                                    Console.WriteLine($"Data copy process completed for {tableName}.");
                                }
                            }
                        }
                        sourceConnection.Close();
                    }
                }
            }
        }

        private SqlDbType GetSqlDbTypeForColumn(string columnName, DataTable schemaTable)
        {
            var row = schemaTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("COLUMN_NAME") == columnName);
            if (row != null)
            {
                var dataType = row.Field<string>("DATA_TYPE").ToLower();
                switch (dataType)
                {
                    case "int":
                        return SqlDbType.Int;
                    case "image":
                        return SqlDbType.VarBinary; // Adjust this if your data type is different
                    case "datetime":
                        return SqlDbType.DateTime;
                    case "varchar":
                        return SqlDbType.VarChar; // Adjust this if your data type is different
                    case "uniqueidentifier":
                        return SqlDbType.UniqueIdentifier;
                    case "decimal":
                        return SqlDbType.Decimal; // Add this case for decimal data type
                    case "xml":
                        return SqlDbType.Xml;// Add more cases for other data types as needed
                    case "bit":
                        return SqlDbType.Bit;
                    default:
                        throw new NotSupportedException($"Unsupported data type for column {columnName}: {dataType}");
                }
            }
            throw new ArgumentException($"Column {columnName} not found in schema table.");
        }

        private int GetSizeForColumn(string columnName, DataTable schemaTable)
        {
            var row = schemaTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("COLUMN_NAME") == columnName);
            if (row != null && row.Table.Columns.Contains("CHARACTER_MAXIMUM_LENGTH"))
            {
                var maxSize = row.Field<int?>("CHARACTER_MAXIMUM_LENGTH");
                return maxSize ?? -1;
            }
            return -1; // Return -1 if size information is not available or not applicable
        }


        private void RestoreDbFromBackUp(string dbBackupFilePath)
        {
            // Set the connection string for the master database
            string? connectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Provide the path to your backup file
            string backupFilePath = dbBackupFilePath;

            // Create a SqlConnection to the master database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlCommand to execute the restore command
                using (SqlCommand command = connection.CreateCommand())
                {
                    // Specify the restore command with the backup file path and target database name
                    //command.CommandText = $"RESTORE DATABASE [{_restoreDbName}] FROM DISK = '{backupFilePath}' WITH REPLACE, STATS = 10";
                    command.CommandText = $"RESTORE DATABASE [{_restoreDbName}] FROM DISK = '{backupFilePath}' " +
                                          $"WITH FILE = 1,  MOVE N'{_restoreDbName}' TO N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{_restoreDbName}_Data.mdf'," +
                                          $"MOVE N'AEWDCS_log' TO N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{_restoreDbName}_Log.ldf',  NOUNLOAD,  STATS = 5";

                    // Execute the restore command
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Database '{_restoreDbName}' restored successfully.");
                connection.Close();
            }
        }

    }
}
