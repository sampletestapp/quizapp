namespace AccessElectionsService.api.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionNumber { get; set; }
        public string Section { get; set; }
        public int QuestionSeverityId { get; set; }
        public QuestionSeverity QuestionSeverity { get; set; }
        public int QuestionTypeId { get; set; }
        public QuestionType QuestionType { get; set; }
        public ICollection<QuestionAnswer> QuestionAnswers { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
