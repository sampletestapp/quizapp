namespace AccessElectionsService.api.Models
{
    public class ResponseResultModel
    {
        public int Id { get; set; } 
        public int SurveyID { get; set; }
        public int? QuestionID { get; set; }
        public string? QuestionText { get; set; }
        public int? QuestionTypeID { get; set; }
        public string? QuestionNumber { get; set; }
        public string? Answers { get; set; }
        public List<string>? AnswerTexts { get; set; }
        public List<string>? QuestionAnswerFindingText { get; set; }
        public string? AdditionalInfo { get; set; }
    }

    public class UpdateResponseResultModel
    {
        public int Id { get; set; }
        public int SurveyID { get; set; }
        public int? QuestionID { get; set; }
        public string? QuestionNumber { get; set; }
        public string? Answers { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
