namespace AccessElectionsService.api.Models
{
    public class FileExportStatsModel
    {
        public string Filename { get; set; }
        public int NoOfTimesFileExported { get; set; }
        public int NoOfFilesStatusCompleted { get; set; }
    }
}
