namespace AccessElectionsService.api.Models
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public bool IsActive { get; set; }
        public string QuestionNumber { get; set; }
        public int SectionId { get; set; }
        public int QuestionSeverityId { get; set; }
        public List<QuestionAnswerDto> QuestionAnswers { get; set; }
        public int ZoneId { get; set; }

    }
}
