using System.ComponentModel.DataAnnotations.Schema;

namespace quizapp.api.Models
{
    public class Recommendation
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}
