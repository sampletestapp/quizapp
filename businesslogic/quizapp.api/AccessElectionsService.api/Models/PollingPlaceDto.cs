using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace AccessElectionsService.api.Models
{
    public class PollingPlaceDto
    {
        public string PollingPlaceIds { get; set; }
        public string ElectionId { get; set; }
        public string UserId{ get; set; }
    }
}
