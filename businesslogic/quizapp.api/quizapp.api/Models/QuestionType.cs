using System.ComponentModel.DataAnnotations.Schema;

namespace quizapp.api.Models
{
    public class QuestionType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public ICollection<Question> Questions { get; set; }

    }
}
