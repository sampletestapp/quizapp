namespace quizapp.api.Models
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionNumber { get; set; }
        public string Section { get; set; }
        public string Zones { get; set; }
        public int QuestionSeverityId { get; set; }
        public List<AnswerDto> Answers { get; set; }
        
    }
}
