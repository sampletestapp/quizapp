using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    [Table("QuestionAnswerFinding", Schema = "AE")]
    public class QuestionAnswerFinding
    {
        public int Id { get; set; }
        public string QuestionAnswerFindingText { get; set; }
        public int QuestionAnswerId { get; set; }
        public QuestionAnswer QuestionAnswer { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
