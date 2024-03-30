using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Services
{
    /// <summary>
    /// This is reponsible in CRUD of survey responses
    /// </summary>
    public interface ISurveyResponseService
    {
        void LoadData(DataHandler dataHandler);
        List<ResponseResultModel> GetResponseForPPLAndElectionId(int pplId, int electionId);
        void UpdatingResponseForQuestion(UpdateResponseResultModel UpdatedRecord);
        void UpdateResponseDashboardAvaialbility(List<UpdateResponseDashboardAvaialbilityModel> records);
        void UpdateSurveyStatus(SurveyStatusUpdateModel surveyStatusUpdate);
        string GetSurveyStatus(int pplId, int electionId);
        List<FileExportStatsModel> GetPollingPlaceSurveyDetails();
    }
}
