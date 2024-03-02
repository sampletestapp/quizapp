using Microsoft.Data.SqlClient;
using System.Data;

namespace AccessElectionsService.api.Repositories
{
    public class ExportRepository : IExportRepository
    {
        private readonly IConfiguration _configuration;
        private string[]? _restoreTableNames;
        private string[]? _identityColumns;
        private string? _restoreDbName;
        private readonly ILogger<ExportRepository> _logger;

        public ExportRepository(IConfiguration configuration, ILogger<ExportRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void GetRestoreTableDbNames()
        {
            _logger.LogDebug("Getting restore table database names.");
            // Get the TablesToRestore section
            var tablesSection = _configuration.GetSection("RestoreInfo");

            // Get the comma-separated table names
            var tableNames = tablesSection.GetValue<string>("Tables");
            var dbName = tablesSection.GetValue<string>("Db");
            var identityColumns = tablesSection.GetValue<string>("IdentityColumns");

            // Split the table names into an array
            _restoreTableNames = tableNames?.Split(',').Select(tableName => tableName.Trim()).ToArray();
            _identityColumns = identityColumns?.Split(',').Select(columnName => columnName.Trim()).ToArray();
            _restoreDbName = dbName?.ToString();
            _logger.LogDebug("Restore table database names retrieved.");
        }

        public void RemoveTheTempDb()
        {
            _logger.LogDebug("Removing the temporary database.");
            // Set the connection string for the master database
            string? masterConnectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Create SqlConnection for the master database
            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                // Set the database to SINGLE_USER mode
                using (SqlCommand setSingleUserCommand = masterConnection.CreateCommand())
                {
                    setSingleUserCommand.CommandText = string.Format(SqlQueries.SetSingleUserQuery, _restoreDbName);
                    setSingleUserCommand.ExecuteNonQuery();
                }

                // Drop the database
                using (SqlCommand dropCommand = masterConnection.CreateCommand())
                {
                    dropCommand.CommandText = string.Format(SqlQueries.DropDatabaseQuery, _restoreDbName);
                    dropCommand.ExecuteNonQuery();
                }
            }
            _logger.LogDebug("Temporary database removed.");
        }
        public void CopyDataToTarget()
        {
            _logger.LogDebug("Copying data to target database.");
            // Set the connection string for the source database (restored database)
            string? sourceConnectionString = _configuration.GetConnectionString("DataSourceConnection");

            // Set the connection string for the target database
            string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

            // List of tables to copy data from
            if (_restoreTableNames?.Length > 0)
            {
                foreach (var tableName in _restoreTableNames)
                {
                    // Create SqlConnection for the source database
                    using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
                    {
                        sourceConnection.Open();

                        // Get column names from the current table in the source database
                        DataTable schemaTable = sourceConnection.GetSchema("Columns", new[] { null, null, tableName, null });
                        string?[] columnNames = schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray().Except(_identityColumns).ToArray();

                        // Create a SqlCommand to select data from the current table
                        using (SqlCommand selectCommand = sourceConnection.CreateCommand())
                        {
                            selectCommand.CommandText = string.Format(SqlQueries.SelectTableQuery, tableName);

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
                                        insertCommand.CommandText = string.Format(SqlQueries.InsertTableQuery, tableName, string.Join(", ", columnNames), string.Join(", ", columnNames.Select(col => "@" + col)));

                                        // Define parameters for the insert command
                                        foreach (var columnName in columnNames)
                                        {
                                            if (columnName != null)
                                            {
                                                // Adjust SqlDbType accordingly based on the data type of the column
                                                SqlDbType sqlDbType = DataHelper.GetSqlDbTypeForColumn(columnName, schemaTable);
                                                insertCommand.Parameters.Add("@" + columnName, sqlDbType, DataHelper.GetSizeForColumn(columnName, schemaTable));
                                            }
                                        }
                                        // Iterate through the source data and insert into the target database
                                        InsertTableData(tableName, columnNames, reader, insertCommand);
                                    }
                                }
                            }
                            sourceConnection.Close();
                        }
                    }
                }
            }
            _logger.LogDebug("Data copied to target database.");
        }

        private void InsertTableData(string tableName, string?[] columnNames, SqlDataReader reader, SqlCommand insertCommand)
        {
            _logger.LogDebug("Data Inserting to target database.");

            while (reader.Read())
            {
                // Set parameter values dynamically
                for (int i = 0; i < columnNames.Length; i++)
                {
                    var columnName = columnNames[i];
                    var columnValue = reader[columnName];

                    // Check for DBNull and handle accordingly
                    insertCommand.Parameters["@" + columnName].Value = columnValue == DBNull.Value ? DBNull.Value : columnValue;
                }

                try
                {
                    // Attempt to execute the insert command
                    insertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Handle the exception
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        // SQL Server error numbers for unique constraint violation
                        Console.WriteLine($"Skipped duplicate record for {tableName}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            _logger.LogDebug("Data Insertied to target database.");
        }

        public void RestoreDbFromBackUp(string dbBackupFilePath)
        {
            _logger.LogDebug("Restoring database from backup.");
            // Set the connection string for the master database
            string? connectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Create a SqlConnection to the master database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlCommand to execute the restore command
                using (SqlCommand command = connection.CreateCommand())
                {
                    // Specify the restore command with the backup file path and target database name
                    command.CommandText = string.Format(SqlQueries.RestoreDatabaseQuery, _restoreDbName, dbBackupFilePath);

                    // Execute the restore command
                    command.ExecuteNonQuery();
                }
                Console.WriteLine($"Database '{_restoreDbName}' restored successfully.");
                connection.Close();
            }
            _logger.LogDebug("Database restore completed.");
        }
    }
}