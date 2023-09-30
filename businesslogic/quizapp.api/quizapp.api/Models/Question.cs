namespace quizapp.api.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string QuestionNumber { get; set; }
        public string Section { get; set; }
        public int QuestionSeverityId { get; set; }
        public QuestionSeverity QuestionSeverity { get; set; }
        public int QuestionTypeId { get; set; }
        public QuestionType QuestionType { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
