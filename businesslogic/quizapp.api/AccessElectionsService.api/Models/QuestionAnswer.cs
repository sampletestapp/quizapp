using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    public class QuestionAnswer
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public ICollection<QuestionAnswerRecommendation> Recommendations { get; set; }
        public ICollection<QuestionAnswerFinding> Findings { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn{ get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
