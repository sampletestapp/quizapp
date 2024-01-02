using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Services
{
    public interface IPollingPlaceService
    {
        Task<PollingPlaceDto> GetPollingPlaceDetails(PollingPlaceDto pollingPlaceDto);
    }
}
