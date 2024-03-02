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
    }
}
