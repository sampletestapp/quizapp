using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Xml;

namespace AccessElectionsService.api.Repositories
{
    public class ProcessingSurveyRepository : IProcessingSurveyRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProcessingSurveyRepository> _logger;
        private const int BlankQuestionType = 1;
        public ProcessingSurveyRepository(IConfiguration configuration, ILogger<ProcessingSurveyRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        public void ProcessSurveys(int electionId)
        {
            _logger.LogInformation("Processing surveys.");
            try
            {
                string? connectionStringTarget = _configuration.GetConnectionString("DataTargetConnection");
                string? connectionStringSource = _configuration.GetConnectionString("DataSourceConnection");

                CreateTargetTableIfNotExists(connectionStringTarget);

                using (SqlConnection readDataConnection = new SqlConnection(connectionStringSource))
                {
                    readDataConnection.Open();

                    using (SqlCommand selectCommand = new SqlCommand(SqlQueries.SelectSurveyResponseQuery, readDataConnection))
                    {
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            ProcessSurveyData(reader, electionId);
                        }
                    }
                }
                _logger.LogInformation("Processed surveys.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing surveys");
                throw;
            }
        }

        private void ProcessSurveyData(SqlDataReader reader, int electionId)
        {
            _logger.LogInformation("Processing survey data");
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
                    _logger.LogInformation("Processied survey data");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing survey with SurveyID {surveyId}");
                    throw;
                }
            }
        }


        private void CreateTargetTableIfNotExists(string? connectionStringTarget)
        {
            _logger.LogInformation("Creating Tables Survey and Response Results");
            try
            {
                using (SqlConnection createTableConnection = new SqlConnection(connectionStringTarget))
                {
                    createTableConnection.Open();
                    using (SqlCommand createTableCommand = new SqlCommand(SqlQueries.CreateSurveyTableQuery, createTableConnection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }
                    using (SqlCommand createTableCommand = new SqlCommand(SqlQueries.CreateResponseResultsTableQuery, createTableConnection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }
                }
                _logger.LogInformation("Created Tables Survey and Response Results");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Creating Tables Survey and Response Results {ex.Message}");
                throw;
            }
        }

        private int SurveyDataProcessing(int pplId, DateTime? conductedDate, int createdUserID, int electionId)
        {
            _logger.LogInformation($"Processing SurveyData for {pplId}, which is conducted on {conductedDate} for electionId {electionId}");
            int insertedRowId = 0;
            try
            {
                // Parse XML
                var connectionString = _configuration.GetConnectionString("DataTargetConnection");

                //Insert data into SQL Server
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(SqlQueries.InsertDataIntoSurveyQuery, connection))
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
                        }
                    }
                }
                _logger.LogInformation($"Processed SurveyData for {pplId}, which is conducted on {conductedDate} for electionId {electionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Processing SurveyData for {pplId}, which is conducted on {conductedDate} for electionId {electionId}, Error: {ex.Message}");
            }
            return insertedRowId;
        }

        private void SurveyResponseResultsProcessing(int surveyId, string xmlData)
        {
            _logger.LogInformation($"Processing Survey Response Results for {surveyId}");
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");

                foreach (XmlNode questionNode in xmlDoc.SelectNodes("//Question"))
                {
                    string? questionNumber = questionNode.Attributes["ID"]?.Value;
                    (int? questionId, int? questionType) = GetQuestionIdAndType(targetConnectionString, questionNumber);
                    List<string> additionalInfoList = new List<string>();
                    List<string?> answersList = new List<string?>();

                    foreach (XmlNode answerNode in questionNode.SelectNodes("Answer"))
                    {
                        string answerText = answerNode.InnerText;
                        string? answer = null;
                        //if question type is blank then answer = answertext, 
                        if (questionType != BlankQuestionType)
                        {
                            answer = GetAnswerIds(targetConnectionString, questionId, answerText);
                            if (answer == null && answerText != null)
                            {
                                additionalInfoList.Add(answerText);
                            }
                            if (answer != null)
                            {
                                answersList.Add(answer);
                            }
                        }
                        else
                        {
                            if (answerText != null)
                            {
                                answersList.Add(answerText);
                            }
                        }
                    }

                    string answers = string.Join(",", answersList.Select(id => id.ToString()).ToArray());
                    string additionalInfos = string.Join(";", additionalInfoList.ToArray());

                    InsertResponseResult(targetConnectionString, surveyId, questionId, questionNumber, answers, additionalInfos);
                    _logger.LogInformation($"Processed Survey Response Results for {surveyId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Processing Survey Response Results for {surveyId} Error: {ex.Message}");
            }
        }

        private string? GetAnswerIds(string? connectionString, int? questionId, string answerText)
        {
            if (!questionId.HasValue)
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(SqlQueries.SelectAnswerFromQuestionAnswersQuery, connection))
                {
                    var cmdText = selectCommand.CommandText;
                    selectCommand.Parameters.AddWithValue("@QuestionID", questionId.Value);
                    selectCommand.Parameters.AddWithValue("@AnswerText", answerText);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(reader.GetOrdinal("ID")).ToString();
                        }
                    }
                }
            }

            return null;
        }


        private (int?,int?) GetQuestionIdAndType(string? connectionString, string? questionNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(SqlQueries.GetQuestionIdAndTypeFromQuestionNumberQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@QuestionID", questionNumber);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (reader.GetInt32(reader.GetOrdinal("Id")), reader.GetInt32(reader.GetOrdinal("QuestionTypeId")));
                        }
                    }
                }
            }

            return (null, null);
        }

        private void InsertResponseResult(string? connectionString, int surveyId, int? questionId, string? questionNumber, string answer, string? additionalInfo)
        {
            //_logger.LogInformation($"Inserting Response Results for {surveyId}");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(SqlQueries.InsertToResponseResultsQuery, connection))
                {
                    command.Parameters.AddWithValue("@SurveyID", surveyId);
                    command.Parameters.AddWithValue("@QuestionID", questionId == null ? DBNull.Value : questionId);
                    command.Parameters.AddWithValue("@QuestionNumber", questionNumber);
                    command.Parameters.AddWithValue("@Answer", answer == null ? DBNull.Value : answer);
                    command.Parameters.AddWithValue("@AdditionalInfo", additionalInfo == null ? DBNull.Value : additionalInfo);
                    //when inserting response will be default null
                    command.Parameters.AddWithValue("@AvailableForDashboard", false);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
