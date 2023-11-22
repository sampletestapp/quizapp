using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    [Table("QuestionSeverity", Schema = "AE")]
    public class QuestionSeverity
    {
        public int Id { get; set; }
        public int QuestionSeverityText { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
