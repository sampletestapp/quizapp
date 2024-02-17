using AccessElectionsService.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AccessElectionsService.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataHandlerController : Controller
    {
        private readonly IConfiguration _configuration;
        private string[]? _restoreTableNames;
        private string[]? _identityColumns;
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
                dataHandler.backupFilePath = "D:\\Work\\Tobedeeleted\\Naren\\test.bak";
                GetRestoreTableDbNames();
                RestoreDbFromBackUp(dataHandler.backupFilePath);
                CopyDataToTarget();
                //Read only from temp
                ProcessSurveys(dataHandler.electionId);
                RemoveTheTempDb();
                return Ok("Data copy process completed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [HttpGet("get-records")]
        public IActionResult GetRecords(int pplId, int electionId)
        {
            try
            {
                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

                List<ResponseResultModel> records = RetrieveRecords(targetConnectionString, pplId, electionId);

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        private List<ResponseResultModel> RetrieveRecords(string connectionString, int pplId, int electionId)
        {
            List<ResponseResultModel> records = new List<ResponseResultModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectRecordsQuery = @"
                                                SELECT rr.*
                                                FROM AE.ResponseResults rr
                                                INNER JOIN AE.Survey s ON rr.SurveyID = s.ID
                                                WHERE s.PPLID = @PPLID AND s.ElectionID = @ElectionID";


                    using (SqlCommand selectCommand = new SqlCommand(selectRecordsQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@PPLID", pplId);
                        selectCommand.Parameters.AddWithValue("@ElectionID", electionId);

                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map data to your model and add to the list
                                ResponseResultModel record = new ResponseResultModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")), // Assuming Id is an integer, adjust accordingly
                                    SurveyID = reader.GetInt32(reader.GetOrdinal("SurveyID")),
                                    QuestionID = reader.IsDBNull(reader.GetOrdinal("QuestionID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuestionID")),
                                    QuestionNumber = reader.IsDBNull(reader.GetOrdinal("QuestionNumber")) ? null : reader.GetString(reader.GetOrdinal("QuestionNumber")),
                                    AnswerID = reader.IsDBNull(reader.GetOrdinal("AnswerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AnswerID")),
                                    AnswerText = reader.IsDBNull(reader.GetOrdinal("AnswerText")) ? null : reader.GetString(reader.GetOrdinal("AnswerText"))
                                    //AnswerText = reader.GetString(reader.GetOrdinal("AnswerText"))
                                };
                                records.Add(record);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error retrieving records: {ex.Message}");
            }

            return records;
        }


        [HttpGet("get-answer-ids")]
        public IActionResult GetAnswerIds(int questionId)
        {
            try
            {
                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

                List<int?> answerIds = RetrieveAnswerIds(targetConnectionString, questionId);

                return Ok(answerIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private List<int?> RetrieveAnswerIds(string connectionString, int questionId)
        {
            List<int?> answerIds = new List<int?>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectAnswerIdsQuery = @"
                        SELECT AnswerID
                        FROM AE.ResponseResults
                        WHERE QuestionID = @QuestionID";

                    using (SqlCommand selectCommand = new SqlCommand(selectAnswerIdsQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@QuestionID", questionId);

                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int? answerId = reader.IsDBNull(reader.GetOrdinal("AnswerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AnswerID"));
                                answerIds.Add(answerId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error retrieving answer IDs: {ex.Message}");
            }

            return answerIds;
        }


        [HttpPost("update-questionanswer")]
        public IActionResult UpdateQuestionAnswer([FromBody] UpdateResponseResultModel updatedRecord)
        {
            try
            {
                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

                // Update the record in the database using the updatedRecord object
                UpdateRecord(targetConnectionString, updatedRecord);

                return Ok("Record saved successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private void UpdateRecord(string connectionString, UpdateResponseResultModel updatedRecord)
        {
            try
            {
                var questionType = GetQuestionType(connectionString,updatedRecord.QuestionNumber);
                if (questionType!=3)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string updateRecordQuery = @"
                                        UPDATE AE.ResponseResults
                                        SET AnswerID = @AnswerID, AnswerText = @AnswerText
                                        WHERE Id = @Id";

                        using (SqlCommand updateCommand = new SqlCommand(updateRecordQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", updatedRecord.Id);
                            updateCommand.Parameters.AddWithValue("@AnswerID", updatedRecord.AnswerID.FirstOrDefault() ?? (object)DBNull.Value);
                            updateCommand.Parameters.AddWithValue("@AnswerText", string.IsNullOrEmpty(updatedRecord.AnswerText) ? (object)DBNull.Value : updatedRecord.AnswerText);

                            updateCommand.ExecuteNonQuery();
                        }
                    } 
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string deleteRecordsQuery = @"
                        DELETE FROM AE.ResponseResults
                        WHERE QuestionID = @QuestionID";

                        using (SqlCommand deleteCommand = new SqlCommand(deleteRecordsQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@QuestionID", updatedRecord.QuestionID);

                            deleteCommand.ExecuteNonQuery();
                        }

                        // Insert new records
                        // Assuming you have a list of new records to insert
                        foreach (var answerId in updatedRecord.AnswerID)
                        {
                            string insertRecordQuery = @"
                            INSERT INTO AE.ResponseResults (SurveyID, QuestionID, AnswerID,QuestionNumber, AnswerText)
                            VALUES (@SurveyID, @QuestionID, @AnswerID,@QuestionNumber, @AnswerText)";

                            using (SqlCommand insertCommand = new SqlCommand(insertRecordQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@SurveyID", updatedRecord.SurveyID);
                                insertCommand.Parameters.AddWithValue("@QuestionID", updatedRecord.QuestionID);
                                insertCommand.Parameters.AddWithValue("@AnswerID", answerId ?? (object)DBNull.Value);
                                insertCommand.Parameters.AddWithValue("@QuestionNumber", updatedRecord.QuestionNumber);
                                insertCommand.Parameters.AddWithValue("@AnswerText", string.IsNullOrEmpty(updatedRecord.AnswerText) ? (object)DBNull.Value : updatedRecord.AnswerText);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error updating record: {ex.Message}");
            }
        }

        private IActionResult ProcessSurveys(int electionId)
        {
            try
            {
                string connectionStringTarget = _configuration.GetConnectionString("DataTargetConnection");
                string connectionStringSource = _configuration.GetConnectionString("DataSourceConnection");

                CreateTargetTableIfNotExists(connectionStringTarget);

                using (SqlConnection readDataConnection = new SqlConnection(connectionStringSource))
                {
                    readDataConnection.Open();

                    string selectQuery = "SELECT SurveyID, PPLID, Response, ConductedDate, CreatedUserID FROM SurveyResponse";

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, readDataConnection))
                    {
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            ProcessSurveyData(reader, electionId);
                        }
                    }
                }

                return Ok("Data processed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing surveys: {ex.Message}");
                throw;
            }
        }

        private void ProcessSurveyData(SqlDataReader reader, int electionId)
        {
            while (reader.Read())
            {
                int pplId = reader.GetInt32(reader.GetOrdinal("PPLID"));
                string xmlData = reader.GetString(reader.GetOrdinal("Response"));
                int createdUserID = reader.GetInt32(reader.GetOrdinal("CreatedUserID"));

                DateTime? conductedDate = reader.IsDBNull(reader.GetOrdinal("ConductedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ConductedDate"));

                int surveyId = SurveyDataProcessing(pplId, conductedDate, createdUserID, electionId);

                try
                {
                    SurveyResponseResultsProcessing(surveyId, xmlData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing surveys: {ex.Message}");
                    throw;
                }
            }
        }


        private static void CreateTargetTableIfNotExists(string? connectionStringTarget)
        {
            try
            {
                using (SqlConnection createTableConnection = new SqlConnection(connectionStringTarget))
                {
                    createTableConnection.Open();
                    string createSurveyTableQuery = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AE' AND TABLE_NAME = 'Survey') " +
                                "CREATE TABLE AE.Survey (" +
                                "    ID INT IDENTITY(1,1) PRIMARY KEY, " +
                                "    PPLID INT, " +
                                "    ElectionID INT, " +
                                "    ConductedDate DATETIME, " +
                                "    CreatedUserID INT, " +
                                "    CONSTRAINT UQ_PPLID_ElectionID UNIQUE (PPLID, ElectionID)" +
                                ")";


                    string createResponseResultsTableQuery = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AE' AND TABLE_NAME = 'ResponseResults') " +
                                                             "CREATE TABLE AE.ResponseResults (" +
                                                             "    ID INT IDENTITY(1,1) PRIMARY KEY, " +
                                                             "    SurveyID INT, " +
                                                             "    QuestionID INT, " +
                                                             "    QuestionNumber VARCHAR(50), " +
                                                             "    AnswerID INT, " +
                                                             "    AnswerText VARCHAR(MAX) " +
                                                             ")";


                    using (SqlCommand createTableCommand = new SqlCommand(createSurveyTableQuery, createTableConnection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }
                    using (SqlCommand createTableCommand = new SqlCommand(createResponseResultsTableQuery, createTableConnection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing : {ex.Message}");
                throw;
            }
        }

        private int SurveyDataProcessing(int pplId, DateTime? conductedDate, int createdUserID, int electionId)
        {
            int insertedRowId = 0;
            try
            {
                // Parse XML
                var connectionString = _configuration.GetConnectionString("DataTargetConnection");

                //Insert data into SQL Server
                string insertQuery = "INSERT INTO AE.Survey (PPLID, ElectionID, ConductedDate, CreatedUserID) " +
                                     "VALUES (@PPLID, @ElectionID, @ConductedDate ,@CreatedUserID);" +
                                     "SELECT SCOPE_IDENTITY();";  // Add this line to retrieve the identity value
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        //seperate the surveyId, PollingId, createdDate, ElectionId to a survey table
                        command.Parameters.AddWithValue("@PPLID", pplId);
                        command.Parameters.AddWithValue("@ElectionID", electionId);
                        command.Parameters.AddWithValue("@ConductedDate", conductedDate == null ? DBNull.Value : conductedDate);
                        command.Parameters.AddWithValue("@CreatedUserID", createdUserID);
                        // ExecuteScalar to get the identity value
                        var identity = command.ExecuteScalar();

                        // Cast the identity value to the appropriate data type
                        if (identity != null && identity != DBNull.Value)
                        {
                            insertedRowId = Convert.ToInt32(identity);
                            // Now you have the identity of the inserted row in the variable 'insertedRowId'
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle parsing or insertion errors
                Console.WriteLine($"Error processing survey with SurveyID {pplId}: {ex.Message}");
            }
            return insertedRowId;
        }


        private void SurveyResponseResultsProcessing(int surveyId, string xmlData)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

                foreach (XmlNode questionNode in xmlDoc.SelectNodes("//Question"))
                {
                    string questionNumber = questionNode.Attributes["ID"].Value;
                    string answerText = questionNode.SelectSingleNode("Answer").InnerText;
                    int? questionId = GetQuestionId(targetConnectionString, questionNumber);
                    int? answerId = GetAnswerId(targetConnectionString, questionId, answerText);

                    InsertResponseResult(targetConnectionString, surveyId, questionId, questionNumber, answerId, answerId == null ? answerText : null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing survey with SurveyID {surveyId}: {ex.Message}");
            }
        }

        private int? GetAnswerId(string connectionString, int? questionId, string answerText)
        {
            if (!questionId.HasValue)
            {
                return null;
            }

            string selectAnswerQuery = "SELECT ID FROM AE.QuestionAnswer WHERE QuestionID = @QuestionID AND QuestionAnswerText = @AnswerText";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectAnswerQuery, connection))
                {
                    var cmdText = selectCommand.CommandText;
                    selectCommand.Parameters.AddWithValue("@QuestionID", questionId.Value);
                    selectCommand.Parameters.AddWithValue("@AnswerText", answerText);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(reader.GetOrdinal("ID"));
                        }
                    }
                }
            }

            return null;
        }


        private int? GetQuestionId(string connectionString, string questionNumber)
        {
            string selectQuestionQuery = "SELECT [Id] FROM AE.Question WHERE [QuestionNumber] = @QuestionID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectQuestionQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@QuestionID", questionNumber);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(reader.GetOrdinal("Id"));
                        }
                    }
                }
            }

            return null;
        }

        private int? GetQuestionType(string connectionString, string questionNumber)
        {
            string selectQuestionQuery = "SELECT [QuestionTypeId] FROM AE.Question WHERE [QuestionNumber] = @QuestionID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectQuestionQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@QuestionID", questionNumber);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(reader.GetOrdinal("QuestionTypeId"));
                        }
                    }
                }
            }

            return null;
        }

        private void InsertResponseResult(string connectionString, int surveyId, int? questionId, string questionNumber, int? answerId, string? answerText)
        {
            string insertQuery = "INSERT INTO AE.ResponseResults (SurveyID, QuestionID, QuestionNumber, AnswerID, AnswerText) " +
                                 "VALUES (@SurveyID, @QuestionID, @QuestionNumber, @AnswerID, @AnswerText)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@SurveyID", surveyId);
                    command.Parameters.AddWithValue("@QuestionID", questionId == null ? DBNull.Value : questionId);
                    command.Parameters.AddWithValue("@QuestionNumber", questionNumber);
                    command.Parameters.AddWithValue("@AnswerID", answerId == null ? DBNull.Value : answerId);
                    command.Parameters.AddWithValue("@AnswerText", answerText == null ? DBNull.Value : answerText);

                    command.ExecuteNonQuery();
                }
            }
        }


        private void GetRestoreTableDbNames()
        {
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
        }

        private void RemoveTheTempDb()
        {
            // Set the connection string for the master database
            string? masterConnectionString = _configuration.GetConnectionString("DataMasterConnection");

            // Create SqlConnection for the master database
            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                // Set the database to SINGLE_USER mode
                using (SqlCommand setSingleUserCommand = masterConnection.CreateCommand())
                {
                    setSingleUserCommand.CommandText = $"USE master; ALTER DATABASE [{_restoreDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    setSingleUserCommand.ExecuteNonQuery();
                }

                // Drop the database
                using (SqlCommand dropCommand = masterConnection.CreateCommand())
                {
                    dropCommand.CommandText = $"USE master; DROP DATABASE [{_restoreDbName}];";
                    dropCommand.ExecuteNonQuery();

                    Console.WriteLine($"Database '{_restoreDbName}' dropped successfully.");
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
                                    insertCommand.CommandText = $"INSERT INTO AE.{tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select(col => $"@{col}"))})";

                                    // Define parameters for the insert command
                                    foreach (var columnName in columnNames)
                                    {
                                        // Adjust SqlDbType accordingly based on the data type of the column
                                        SqlDbType sqlDbType = GetSqlDbTypeForColumn(columnName, schemaTable);
                                        insertCommand.Parameters.Add($"@{columnName}", sqlDbType, GetSizeForColumn(columnName, schemaTable));
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
                                                Console.WriteLine($"Skipped duplicate record for {tableName}");
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
