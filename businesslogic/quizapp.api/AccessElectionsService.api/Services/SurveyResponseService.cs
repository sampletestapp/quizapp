using AccessElectionsService.api.Models;
using AccessElectionsService.api.Repositories;

namespace AccessElectionsService.api.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly IExportRepository _repository;
        private readonly IProcessingSurveyRepository _processingSurveyRepository;
        private readonly IResponseResultRepository _responseResultRepository;
        private readonly ILogger<SurveyResponseService> _logger;

        public SurveyResponseService(IExportRepository repository, IProcessingSurveyRepository processingSurveyRepository,
                                        IResponseResultRepository responseResultRepository, ILogger<SurveyResponseService> logger)
        {
            _repository = repository;
            _responseResultRepository = responseResultRepository;
            _processingSurveyRepository = processingSurveyRepository;
            _logger = logger;
        }

        public List<ResponseResultModel> GetResponseForPPLAndElectionId(int pplId, int electionId)
        {
            return _responseResultRepository.GetResponseForPPLAndElectionId(pplId, electionId);
        }

        public void UpdatingResponseForQuestion(UpdateResponseResultModel UpdatedRecord)
        {
            _responseResultRepository.UpdatingResponseForQuestion(UpdatedRecord);
        }

        public void UpdateResponseDashboardAvaialbility(List<UpdateResponseDashboardAvaialbilityModel> records)
        {
            _responseResultRepository.UpdateResponseDashboardAvaialbility(records);
        }

        public void LoadData(DataHandler dataHandler)
        {
            _logger.LogDebug($"Loading Survey Response service");
            _repository.GetRestoreTableDbNames();
            _repository.RestoreDbFromBackUp(dataHandler.backupFilePath);
            _repository.CopyDataToTarget();
            _processingSurveyRepository.ProcessSurveys(dataHandler.electionId);
            _repository.RemoveTheTempDb();
            _logger.LogDebug($"Loading Survey Response service completed");
        }
    }
}
