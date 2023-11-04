namespace quizapp.api.Models
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionNumber { get; set; }
        public string Text { get; set; }
        public int QuestionSeverityId { get; set; }
        public int SectionId { get; set; }
        public int ZoneId { get; set; }
        public int QuestionTypeId { get; set; }
        public List<AnswerDto> Answers { get; set; }
        
    }
}
