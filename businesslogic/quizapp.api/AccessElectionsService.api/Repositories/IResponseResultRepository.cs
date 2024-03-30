using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Repositories
{
    public interface IResponseResultRepository
    {
        List<ResponseResultModel> GetResponseForPPLAndElectionId(int pplId, int electionId);
        void UpdatingResponseForQuestion(UpdateResponseResultModel UpdatedRecord);
        void UpdateResponseDashboardAvaialbility(List<UpdateResponseDashboardAvaialbilityModel> Records);
        void UpdateSurveyStatus(SurveyStatusUpdateModel surveyStatusUpdate);
        string GetSurveyStatus(int pplId, int electionId);
        List<FileExportStatsModel> GetPollingPlaceSurveyDetails();
    }
}
