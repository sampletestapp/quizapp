using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    public class QuestionType
    {
        public int Id { get; set; }
        public string QuestionTypeText { get; set; }
        public ICollection<Question> Questions { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
