using Microsoft.AspNetCore.Http;

namespace AccessElectionsService.api.Repositories
{
    public static class SqlQueries
    {
        public const string SetSingleUserQuery = "USE master; ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
        public const string DropDatabaseQuery = "USE master; DROP DATABASE [{0}];";
        public const string SelectTableQuery = "SELECT * FROM {0};";
        public const string InsertTableQuery = "INSERT INTO AE.{0} ({1}) VALUES ({2});";


        //processing data queries
        public const string SelectSurveyResponseQuery = "SELECT SurveyID, PPLID, Response, ConductedDate, CreatedUserID FROM SurveyResponse";

        public const string CreateSurveyTableQuery = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AE' AND TABLE_NAME = 'Survey') " +
                                                     "CREATE TABLE AE.Survey (" +
                                                     "    ID INT IDENTITY(1,1) PRIMARY KEY, " +
                                                     "    PPLID INT, " +
                                                     "    ElectionID INT, " +
                                                     "    ConductedDate DATETIME, " +
                                                     "    CreatedUserID INT, " +
                                                     "    CONSTRAINT UQ_PPLID_ElectionID UNIQUE (PPLID, ElectionID)" +
                                                     ")";


        public const  string CreateResponseResultsTableQuery = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AE' AND TABLE_NAME = 'ResponseResults') " +
                                                               "CREATE TABLE AE.ResponseResults (" +
                                                               "    ID INT IDENTITY(1,1) PRIMARY KEY, " +
                                                               "    SurveyID INT, " +
                                                               "    QuestionID INT, " +
                                                               "    QuestionNumber VARCHAR(50), " +
                                                               "    Answer VARCHAR(50), " +
                                                               "    AdditionalInfo VARCHAR(MAX) " +
                                                               ")";

        public const string InsertDataIntoSurveyQuery = "INSERT INTO AE.Survey (PPLID, ElectionID, ConductedDate, CreatedUserID) " +
                                                        "VALUES (@PPLID, @ElectionID, @ConductedDate ,@CreatedUserID);" +
                                                        "SELECT SCOPE_IDENTITY();";

        public const string SelectAnswerFromQuestionAnswersQuery = "SELECT ID FROM AE.QuestionAnswer WHERE QuestionID = @QuestionID AND QuestionAnswerText = @AnswerText";

        public const string GetQuestionIdAndTypeFromQuestionNumberQuery = "SELECT [Id], [QuestionTypeId] FROM AE.Question WHERE [QuestionNumber] = @QuestionID";

        public const string GetQuestionTypeFromNumberQuery = "SELECT [QuestionTypeId] FROM AE.Question WHERE [QuestionNumber] = @QuestionID";

        public const string InsertToResponseResultsQuery = "INSERT INTO AE.ResponseResults (SurveyID, QuestionID, QuestionNumber, Answer, AdditionalInfo) " +
                                                           "VALUES (@SurveyID, @QuestionID, @QuestionNumber, @Answer, @AdditionalInfo)";

        public const string SelectResponseResultsForPPlIdAndElectionIdQuery = @"
                                            SELECT rr.*,
                                                  q.QuestionTypeId,
                                                  q.QuestionText,
                                                  STUFF((
                                                    SELECT ';' + qa.QuestionAnswerText
                                                    FROM AE.QuestionAnswer qa
                                                    WHERE CHARINDEX(',' + CAST(qa.Id AS VARCHAR) + ',', ',' + rr.Answer + ',') > 0
                                                    FOR XML PATH('')
                                                  ), 1, 1, '') AS QuestionAnswerText,
                                                  STUFF((
                                                    SELECT ';' + qaf.QuestionAnswerFindingText
                                                    FROM AE.QuestionAnswer qa
                                                    LEFT JOIN AE.QuestionAnswerFinding qaf ON qa.Id = qaf.QuestionAnswerId
                                                    WHERE CHARINDEX(',' + CAST(qa.Id AS VARCHAR) + ',', ',' + rr.Answer + ',') > 0
                                                    ORDER BY qaf.Id ASC
                                                    FOR XML PATH('')
                                                  ), 1, 1, '') AS QuestionAnswerFindingText
                                                FROM AE.ResponseResults rr
                                                INNER JOIN AE.Survey s ON rr.SurveyID = s.ID
                                                INNER JOIN AE.Question q ON rr.QuestionID = q.ID
                                                WHERE s.PPLID = @PPLID AND s.ElectionID = @ElectionID";

        public const string UpdateSurveyResponseRecordQuery = @"
                                        UPDATE AE.ResponseResults
                                        SET Answer = @Answer, AdditionalInfo = @AdditionalInfo
                                        WHERE Id = @Id";

        //public const string RestoreDatabaseQuery = $"RESTORE DATABASE [{_restoreDbName}] FROM DISK = '{backupFilePath}' WITH REPLACE, STATS = 10";
        public const string RestoreDatabaseQuery = "RESTORE DATABASE [{0}] FROM DISK = '{1}' " +
                                                   "WITH FILE = 1,  MOVE N'{0}' TO N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{0}_Data.mdf'," +
                                                   "MOVE N'AEWDCS_log' TO N'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{0}_Log.ldf',  NOUNLOAD,  STATS = 5";
    }
}
