namespace AccessElectionsService.api.Models
{
    public class QuestionAnswerDto
    {
        public int Id { get; set; }
        public string QuestionAnswerText { get; set; }
        public List<QuestionAnswerRecommendationDto> Recommendations { get; set; }
        public List<QuestionAnswerFindingDto> Findings { get; set; }
    }
}
