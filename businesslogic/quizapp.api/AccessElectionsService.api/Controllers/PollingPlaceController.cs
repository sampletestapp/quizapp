using Microsoft.AspNetCore.Mvc;
using AccessElectionsService.api.Models;
using AccessElectionsService.api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;

namespace AccessElectionsService.api.Controllers
{
    public class PollingPlaceController : ControllerBase
    {
        private readonly IPollingPlaceService _pollingPlaceService;

        public PollingPlaceController(IPollingPlaceService pollingPlaceService)
        {
            _pollingPlaceService = pollingPlaceService;                 
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetPollingPlaceDetails(PollingPlaceDto PollingPlaceDTO)
        {
            string pollingPlaceDetails = string.Empty;
            List<string> PPLocations = new List<string>();
            StringBuilder fileContent = new StringBuilder();


            //try
            //{
            //    var createdQuestion = await _pollingPlaceService.CreateQuestion(questionDto);
            //    return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
            //}
            //catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex, out var errorCode))
            //{
            //    return HandleUniqueConstraintViolation(errorCode, questionDto);
            //}




            //string PollingPlaceIds = "6CAE9DC6-16DF-E911-8112-0050568C77F8, 6E26E6F9-16DF-E911-8112-0050568C77F8, 71516D1C-17DF-E911-8112-0050568C77F8, CA9897D0-17DF-E911-8112-0050568C77F8";
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




                //using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["EDM_MSCRM_EXTConnectionString"].ConnectionString))
                //{
                //    sqlConnection.Open();

                //    using (SqlCommand sqlCommand = new SqlCommand("PrGetPollingPlaceDetails", sqlConnection))
                //    {
                //        sqlCommand.CommandType = CommandType.StoredProcedure;
                //        sqlCommand.Parameters.AddWithValue("@PollingPlaceIds", PollingPlaceIds);
                //        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                //        {
                //            if (reader != null)
                //            {
                //                while (reader.Read())
                //                {
                //                    fileContent.Append(System.Environment.NewLine);
                //                    fileContent.Append("(");
                //                    fileContent.Append("'" + reader["Description"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append(reader["Hindi"] + ",");
                //                    fileContent.Append("'" + reader["StatusCode"] + "',");
                //                    fileContent.Append("'" + reader["AddressLine1"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append("'" + reader["City"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append("'" + reader["StateCode"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append(reader["PostalCode"] + ",");
                //                    fileContent.Append(reader["CountyID"] + ",");
                //                    fileContent.Append("'" + reader["County"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append(reader["MunicipalityID"] + ",");
                //                    fileContent.Append("'" + reader["Municipality"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append(reader["Latitude"] + ",");
                //                    fileContent.Append(reader["Longitude"] + ",");
                //                    fileContent.Append(reader["LastModifiedOn"] + ",");
                //                    fileContent.Append(reader["CreatedOn"] + ",");
                //                    fileContent.Append("'" + reader["BldgClassificationCode"].ToString().Replace("'", "\''") + "',");
                //                    fileContent.Append(reader["PollingPlaceCategoryId"].ToString().Replace("'", "\''") + ",");
                //                    fileContent.Append(reader["PPLID"] + "),");
                //                    PPLocations.Add(reader["PPLID"].ToString());
                //                }
                //            }
                //            fileContent.Append(")");
                //        }
                //        fileContent.Replace("()", "");
                //        fileContent.Length = fileContent.Length - 2;
                //    }
                //}

            //    fileContent.Append(System.Environment.NewLine);
            //    fileContent.Append("INSERT INTO SVRS_PollingPlaceUser").Append(System.Environment.NewLine);
            //    fileContent.Append("(PPLID, UserId)").Append(System.Environment.NewLine);
            //    fileContent.Append("VALUES").Append(System.Environment.NewLine);

            //    foreach (string location in PPLocations)
            //    {
            //        fileContent.Append("(" + location + ",3992),").Append(System.Environment.NewLine);
            //    }
            //    fileContent.Length = fileContent.Length - 3;
            //    string filePath = string.Empty;


            //    using (var xContext = new XrmServiceContext(new CrmUtilities().CrmService))
            //    {
            //        edm_elections election = (from e in xContext.edm_electionsSet where e.edm_electionsId == new Guid(ElectionId) select e).FirstOrDefault();

            //        edm_pollingplacesurvey pplsurvey = new edm_pollingplacesurvey();
            //        pplsurvey.edm_ElectionId = new Microsoft.Xrm.Sdk.EntityReference(edm_elections.EntityLogicalName, new Guid(ElectionId));
            //        pplsurvey.edm_ExportedOn = DateTime.Now;
            //        pplsurvey.edm_ExportedSQL = fileContent.ToString();
            //        pplsurvey.edm_ExportedBy = new Microsoft.Xrm.Sdk.EntityReference(SystemUser.EntityLogicalName, new Guid(UserId));
            //        xContext.AddObject(pplsurvey);
            //        xContext.SaveChanges();


            //        string[] pollingPlaces = PollingPlaceIds.Split(',');
            //        foreach (string pp in pollingPlaces)
            //        {
            //            edm_pollingplacelocations pollingplacelocation = (from ppl in xContext.edm_pollingplacelocationsSet
            //                                                              where ppl.edm_pollingplacelocationsId == new Guid(pp)
            //                                                              select ppl).FirstOrDefault();

            //            edm_pollingplacesurveyresponse pplsurveyresponse = new edm_pollingplacesurveyresponse();
            //            pplsurveyresponse.edm_name = election.edm_name + " - " + pollingplacelocation.edm_name;
            //            pplsurveyresponse.edm_PollingPlaceSurveyId = new Microsoft.Xrm.Sdk.EntityReference(edm_pollingplacesurvey.EntityLogicalName, pplsurvey.Id);
            //            pplsurveyresponse.edm_PollingPlaceId = new Microsoft.Xrm.Sdk.EntityReference(edm_pollingplacelocations.EntityLogicalName, new Guid(pp));


            //            //AssignRequest assignReq = new AssignRequest()
            //            //{
            //            //    Assignee = new EntityReference
            //            //    {
            //            //        LogicalName = edm_pollingplacesurveyresponse.EntityLogicalName,
            //            //        Id = assigneeID
            //            //    },
            //            //    Target = new EntityReference(TargetEntityName, TargetID)
            //            //}; 

            //            //xContext.Execute(assignReq);

            //            //if (pollingplacelocation.OwnerId != null)
            //            //    pplsurveyresponse.OwnerId = new EntityReference();
            //            //    pplsurveyresponse.OwnerId = pollingplacelocation.OwningTeam;
            //            //    pplsurveyresponse.OwnerId.LogicalName = Team.EntityLogicalName;
            //            //pplsurveyresponse.OwnerId = pollingplacelocation.OwnerId;

            //            xContext.AddObject(pplsurveyresponse);
            //            xContext.SaveChanges();

            //            //This is to assign the owner of the polling place survey
            //            //VoterBLL.AssignRequest(xContext, Team.EntityLogicalName, pollingplacelocation.OwningTeam.Id, edm_pollingplacesurveyresponse.EntityLogicalName, pplsurveyresponse.Id);


            //        }
            //    }
            }
            catch (Exception ex)
            {
                //LogBLL.SendEmailTemp("PPL Export Request :" + ex.Message);
            }

            //return fileContent.ToString();
            return pollingPlaceDetails;
        }
    }
}
