namespace AccessElectionsService.api.Repositories
{
    public interface IExportRepository
    {
        void GetRestoreTableDbNames();
        void RemoveTheTempDb();
        void CopyDataToTarget();
        void RestoreDbFromBackUp(string dbBackupFilePath);
    }
}
