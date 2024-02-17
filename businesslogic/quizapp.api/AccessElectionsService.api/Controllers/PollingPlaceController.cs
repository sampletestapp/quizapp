using Microsoft.AspNetCore.Mvc;
using AccessElectionsService.api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;

namespace AccessElectionsService.api.Controllers
{
    public class PollingPlaceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PollingPlaceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<ActionResult<string>> GetPollingPlaceDetails(PollingPlaceDto pollingPlaceDTO)
        {
            string pollingPlaceDetails = string.Empty;
            List<string> PPLocations = new List<string>();
            StringBuilder fileContent = new StringBuilder();
            var connectionString = _configuration.GetConnectionString("DataTargetConnection");

            try
            {

                fileContent.Append("use aewdcs;").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SVRS_PollingPlace Where PPLID NOT IN(593215)").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SVRS_PollingPlaceUser Where PPLID NOT IN(593215)").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SurveyResponseComments").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM PollingPlacePhoto").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SupplyGrant").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SurveyResponseHistory").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM SurveyResponse").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM ActivityLog").Append(System.Environment.NewLine);
                fileContent.Append("DELETE FROM FactSurveyResponse").Append(System.Environment.NewLine).Append(System.Environment.NewLine);


                fileContent.Append("INSERT INTO dbo.SVRS_PollingPlace").Append(System.Environment.NewLine);
                fileContent.Append("(Description, HINDI, StatusCode, AddressLine1, City, StateCode, PostalCode, CountyID, County, MunicipalityID, Municipality, Latitude, Longitude, LastEditDate, CreationDate, BldgClassificationCode, PollingPlaceCAtegoryID, PPLID)").Append(System.Environment.NewLine);
                fileContent.Append("VALUES");


                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("PrGetPollingPlaceDetails", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@PollingPlaceIds", pollingPlaceDTO.PollingPlaceIds);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                while (reader.Read())
                                {
                                    fileContent.Append(System.Environment.NewLine);
                                    fileContent.Append("(");
                                    fileContent.Append("'" + reader["Description"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append(reader["Hindi"] + ",");
                                    fileContent.Append("'" + reader["StatusCode"] + "',");
                                    fileContent.Append("'" + reader["AddressLine1"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append("'" + reader["City"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append("'" + reader["StateCode"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append(reader["PostalCode"] + ",");
                                    fileContent.Append(reader["CountyID"] + ",");
                                    fileContent.Append("'" + reader["County"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append(reader["MunicipalityID"] + ",");
                                    fileContent.Append("'" + reader["Municipality"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append(reader["Latitude"] + ",");
                                    fileContent.Append(reader["Longitude"] + ",");
                                    fileContent.Append(reader["LastModifiedOn"] + ",");
                                    fileContent.Append(reader["CreatedOn"] + ",");
                                    fileContent.Append("'" + reader["BldgClassificationCode"].ToString().Replace("'", "\''") + "',");
                                    fileContent.Append(reader["PollingPlaceCategoryId"].ToString().Replace("'", "\''") + ",");
                                    fileContent.Append(reader["PPLID"] + "),");
                                    PPLocations.Add(reader["PPLID"].ToString());
                                }
                            }
                            fileContent.Append(")");
                        }
                        fileContent.Replace("()", "");
                        fileContent.Length = fileContent.Length - 2;
                    }
                }

                fileContent.Append(System.Environment.NewLine);
                fileContent.Append("INSERT INTO SVRS_PollingPlaceUser").Append(System.Environment.NewLine);
                fileContent.Append("(PPLID, UserId)").Append(System.Environment.NewLine);
                fileContent.Append("VALUES").Append(System.Environment.NewLine);

                foreach (string location in PPLocations)
                {
                    fileContent.Append("(" + location + ",3992),").Append(System.Environment.NewLine);
                }
                fileContent.Length = fileContent.Length - 3;
                string filePath = string.Empty;


                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    // Inserting polling place survey record
                    string insertSurveyQuery = @"INSERT INTO edm_pollingplacesurvey (edm_ElectionId, edm_ExportedOn, edm_ExportedSQL, edm_ExportedBy)
                                     VALUES (@ElectionId, @ExportedOn, @ExportedSQL, @ExportedBy);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand sqlCommand = new SqlCommand(insertSurveyQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@ElectionId", new Guid(pollingPlaceDTO.ElectionId));
                        sqlCommand.Parameters.AddWithValue("@ExportedOn", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@ExportedSQL", fileContent.ToString());
                        sqlCommand.Parameters.AddWithValue("@ExportedBy", new Guid(pollingPlaceDTO.UserId));

                        // Get the inserted survey record's ID
                        int surveyId = Convert.ToInt32(sqlCommand.ExecuteScalar());

                        // Inserting polling place survey response records
                        string[] pollingPlaces = pollingPlaceDTO.PollingPlaceIds.Split(',');
                        foreach (string pp in pollingPlaces)
                        {
                            string insertResponseQuery = @"INSERT INTO edm_pollingplacesurveyresponse (edm_name, edm_PollingPlaceSurveyId, edm_PollingPlaceId)
                                               VALUES (@Name, @SurveyId, @PollingPlaceId);";

                            using (SqlCommand responseCommand = new SqlCommand(insertResponseQuery, sqlConnection))
                            {
                                // Fetching polling place details
                                string selectPlaceQuery = "SELECT edm_name FROM edm_pollingplacelocations WHERE edm_pollingplacelocationsId = @PollingPlaceId;";
                                using (SqlCommand selectCommand = new SqlCommand(selectPlaceQuery, sqlConnection))
                                {
                                    selectCommand.Parameters.AddWithValue("@PollingPlaceId", new Guid(pp));
                                    string pollingPlaceName = selectCommand.ExecuteScalar().ToString();
                                    string selectElectionNameQuery = "SELECT edm_name FROM edm_pollingplacelocations WHERE edm_pollingplacelocationsId = @ElectionId;";

                                    using (SqlCommand selectElectionCommandCommand = new SqlCommand(selectPlaceQuery, sqlConnection))
                                    {
                                        selectElectionCommandCommand.Parameters.AddWithValue("@ElectionId", new Guid(pollingPlaceDTO.ElectionId));
                                        string electionName = selectElectionCommandCommand.ExecuteScalar().ToString();

                                        responseCommand.Parameters.AddWithValue("@Name", electionName + " - " + pollingPlaceName);
                                        responseCommand.Parameters.AddWithValue("@SurveyId", surveyId);
                                        responseCommand.Parameters.AddWithValue("@PollingPlaceId", new Guid(pp));
                                    }
                                    responseCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //LogBLL.SendEmailTemp("PPL Export Request :" + ex.Message);
            }

            return fileContent.ToString();
        }
    }
}
