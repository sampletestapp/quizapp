using AccessElectionsService.api.Models;
using Microsoft.Data.SqlClient;

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
                                    QuestionTypeID = reader.IsDBNull(reader.GetOrdinal("QuestionTypeId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuestionTypeId")),
                                    QuestionNumber = reader.IsDBNull(reader.GetOrdinal("QuestionNumber")) ? null : reader.GetString(reader.GetOrdinal("QuestionNumber")),
                                    Answers = reader.IsDBNull(reader.GetOrdinal("Answer")) ? null : reader.GetString(reader.GetOrdinal("Answer")),
                                    AdditionalInfo = reader.IsDBNull(reader.GetOrdinal("AdditionalInfo")) ? null : reader.GetString(reader.GetOrdinal("AdditionalInfo"))
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
    }
}