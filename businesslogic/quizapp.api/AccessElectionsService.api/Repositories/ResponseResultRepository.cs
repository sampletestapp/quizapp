using AccessElectionsService.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace AccessElectionsService.api.Repositories
{
    public class ResponseResultRepository : IResponseResultRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResponseResultRepository> _logger;


        public ResponseResultRepository(IConfiguration configuration, ILogger<ResponseResultRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public List<ResponseResultModel> GetResponseForPPLAndElectionId(int pplId, int electionId)
        {
            _logger.LogDebug("Getting Response For PPLId And ElectionId");
            List<ResponseResultModel> records = new List<ResponseResultModel>();
            var targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    connection.Open();

                    using (SqlCommand selectCommand = new SqlCommand(SqlQueries.SelectResponseResultsForPPlIdAndElectionIdQuery, connection))
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
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    SurveyID = reader.GetInt32(reader.GetOrdinal("SurveyID")),
                                    QuestionID = reader.IsDBNull(reader.GetOrdinal("QuestionID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuestionID")),
                                    QuestionText = reader.IsDBNull(reader.GetOrdinal("QuestionText")) ? null : reader.GetString(reader.GetOrdinal("QuestionText")),
                                    QuestionTypeID = reader.IsDBNull(reader.GetOrdinal("QuestionTypeId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuestionTypeId")),
                                    QuestionNumber = reader.IsDBNull(reader.GetOrdinal("QuestionNumber")) ? null : reader.GetString(reader.GetOrdinal("QuestionNumber")),
                                    Answers = reader.IsDBNull(reader.GetOrdinal("Answer")) ? null : reader.GetString(reader.GetOrdinal("Answer")),
                                    AnswerTexts = reader.IsDBNull(reader.GetOrdinal("QuestionAnswerText")) ? null : reader.GetString(reader.GetOrdinal("QuestionAnswerText")).Split(';').ToList(),
                                    QuestionAnswerFindingText = reader.IsDBNull(reader.GetOrdinal("QuestionAnswerFindingText")) ? null : reader.GetString(reader.GetOrdinal("QuestionAnswerFindingText")).Split(';').ToList(),
                                    AdditionalInfo = reader.IsDBNull(reader.GetOrdinal("AdditionalInfo")) ? null : reader.GetString(reader.GetOrdinal("AdditionalInfo")),
                                    AvailableForDashboard = reader.IsDBNull(reader.GetOrdinal("AvailableForDashboard")) ? false : reader.GetBoolean(reader.GetOrdinal("AvailableForDashboard"))
                                };
                                records.Add(record);
                            }
                        }
                    }
                }
                _logger.LogDebug("Obtained Response For PPLId And ElectionId");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving records: {ex.Message}");
            }

            return records;
        }

        public void UpdatingResponseForQuestion(UpdateResponseResultModel updatedRecord)
        {
            _logger.LogDebug($"Updating Survey resoponse {updatedRecord.QuestionID}");
            try
            {
                string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
                UpdateRecord(targetConnectionString, updatedRecord);
                _logger.LogDebug($"Updated Survey resoponse {updatedRecord.QuestionID}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Survey resoponse Exception: {ex.Message}");
            }
        }


        public void UpdateResponseDashboardAvaialbility(List<UpdateResponseDashboardAvaialbilityModel> Records)
        {
            _logger.LogDebug($"Updating Response Dashboard Avaialbility");
            try
            {
                string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    connection.Open();
                    foreach (var record in Records)
                    {
                        using (SqlCommand command = new SqlCommand(SqlQueries.UpdateDashboardAvailabilityQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AvailableForDashboard", record.AvailableForDashboard);
                            command.Parameters.AddWithValue("@Id", record.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                _logger.LogDebug($"Updating Response Dashboard Avaialbility");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Response Dashboard Avaialbility Exception: {ex.Message}");
            }
        }


        public void UpdateSurveyStatus(SurveyStatusUpdateModel surveyStatusUpdate)
        {
            _logger.LogDebug($"Updating Survey Status");
            try
            {
                string? targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SqlQueries.UpdateSurveyStatusQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Status", surveyStatusUpdate.Status);
                        command.Parameters.AddWithValue("@PPLID", surveyStatusUpdate.PplId);
                        command.Parameters.AddWithValue("@ElectionID", surveyStatusUpdate.ElectionId);
                        command.ExecuteNonQuery();
                    }
                }

                _logger.LogDebug($"Updated Survey Status");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Survey Status Exception: {ex.Message}");
            }
        }

        private void UpdateRecord(string? connectionString, UpdateResponseResultModel updatedRecord)
        {
            _logger.LogDebug($"Updating Survey resoponse record {updatedRecord.QuestionID}");
            try
            {
               using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand updateCommand = new SqlCommand(SqlQueries.UpdateSurveyResponseRecordQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Id", updatedRecord.Id);
                        updateCommand.Parameters.AddWithValue("@Answer", updatedRecord.Answers ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@AdditionalInfo", string.IsNullOrEmpty(updatedRecord.AdditionalInfo) ? (object)DBNull.Value : updatedRecord.AdditionalInfo);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                _logger.LogDebug($"Updated Survey resoponse record {updatedRecord.QuestionID}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Survey resoponse records Exception: {ex.Message}");
            }
        }

        public string GetSurveyStatus(int pplId, int electionId)
        {
            _logger.LogDebug($"Retrieving Survey Status");
            string status = string.Empty;
            try
            {
                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SqlQueries.GetSurveyStatusQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PPLID", pplId);
                        command.Parameters.AddWithValue("@ElectionID", electionId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                status = reader["Status"].ToString();
                            }
                        }
                    }
                }

                _logger.LogDebug($"Retrieved Survey Status: {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Retrieving Survey Status Exception: {ex.Message}");
            }

            return status;
        }


        public List<FileExportStatsModel> GetPollingPlaceSurveyDetails()
        {
            _logger.LogDebug($"GetPolling PlaceSurvey Details");
            List<FileExportStatsModel> FileExportStats = new List<FileExportStatsModel>();
            try
            {
                string targetConnectionString = _configuration.GetConnectionString("DataTargetConnection");
                using (SqlConnection connection = new SqlConnection(targetConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(SqlQueries.GetPollingPlaceSurveyDetails, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FileExportStats.Add(new FileExportStatsModel
                                {
                                    Filename = reader["Filename"].ToString(),
                                    NoOfTimesFileExported = Convert.ToInt32(reader["NoOfTimesFileExported"]),
                                    NoOfFilesStatusCompleted = 5
                                });
                            }
                        }
                    }
                    _logger.LogDebug($"GetPolling PlaceSurvey Details");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Retrieving GetPolling PlaceSurvey Details Exception: {ex.Message}");
            }

            return FileExportStats;
        }
    }
}