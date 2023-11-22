using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    [Table("Question", Schema = "AE")]
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public bool IsActive { get; set; }
        public string QuestionNumber { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public int QuestionSeverityId { get; set; }
        public QuestionSeverity QuestionSeverity { get; set; }
        public int QuestionTypeId { get; set; }
        public QuestionType QuestionType { get; set; }
        public ICollection<QuestionAnswer> QuestionAnswers { get; set; }
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
