using Microsoft.EntityFrameworkCore;
using quizapp.api.Models;

namespace quizapp.api.Data
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Finding> Findings { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
