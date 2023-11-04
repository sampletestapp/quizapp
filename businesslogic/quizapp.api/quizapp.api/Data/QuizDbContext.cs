using Microsoft.EntityFrameworkCore;
using quizapp.api.Models;

namespace quizapp.api.Data
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionSeverity> QuestionSeverity { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Finding> Findings { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Section> Sections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuizDbContext).Assembly);

            // Assuming you have a DbContext named AppDbContext
            for (char sectionName = 'A'; sectionName <= 'Z'; sectionName++)
            {
                modelBuilder.Entity<Section>().HasData(
                    new Section() { Id = sectionName - 'A' + 1, Name = sectionName.ToString() }
                );
            }
            for (int severity = 1; severity <= 3; severity++)
            {
                modelBuilder.Entity<QuestionSeverity>().HasData(
                    new QuestionSeverity() { Id = severity, Severity = severity }
                );
            }

            modelBuilder.Entity<QuestionType>().HasData(
            new QuestionType() { Id = 1, TypeName = "blank" },
            new QuestionType() { Id = 2, TypeName = "singleSelection" },
            new QuestionType() { Id = 3, TypeName = "multipleChoiceSelection" }
            );

            modelBuilder.Entity<Zone>().HasData(
            new Zone() { Id = 1, Name = "PARKING" },
            new Zone() { Id = 2, Name = "PATHWAYS" },
            new Zone() { Id = 3, Name = "ACCESSIBLE_ENTERANCE" },
            new Zone() { Id = 4, Name = "INTERIOR_ROUTES" },
            new Zone() { Id = 5, Name = "VOTING_AREAS" }
            );
        }
    }
}
