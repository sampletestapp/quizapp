using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Repositories
{
    public interface IProcessingSurveyRepository
    {
        void ProcessSurveys(int electionId);
    }
}
