namespace quizapp.api.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionNumber { get; set; }
        public string Text { get; set; }
        public int QuestionSeverityId { get; set; }
        public QuestionSeverity QuestionSeverity { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public int QuestionTypeId { get; set; }
        public QuestionType QuestionType { get; set; }
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
