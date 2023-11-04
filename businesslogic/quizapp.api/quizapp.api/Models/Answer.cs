using System.ComponentModel.DataAnnotations.Schema;

namespace quizapp.api.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public ICollection<Recommendation> Recommendations { get; set; }
        public Finding Findings { get; set; }
    }
}
