namespace quizapp.api.Models
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<RecommendationDto> Recommendations { get; set; }
        public List<FindingDto> Findings { get; set; }
    }
}
