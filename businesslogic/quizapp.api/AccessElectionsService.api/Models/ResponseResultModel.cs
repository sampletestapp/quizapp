namespace AccessElectionsService.api.Models
{
    public class ResponseResultModel
    {
        public int Id { get; set; } 
        public int SurveyID { get; set; }
        public int? QuestionID { get; set; }
        public string? QuestionNumber { get; set; }
        public string? Answers { get; set; }
        public string? AdditionalInfo { get; set; }
    }

    public class UpdateResponseResultModel
    {
        public int Id { get; set; }
        public int SurveyID { get; set; }
        public int? QuestionID { get; set; }
        public string? QuestionNumber { get; set; }
        public int?[]? AnswerID { get; set; }
        public string? AnswerText { get; set; }
    }
}
