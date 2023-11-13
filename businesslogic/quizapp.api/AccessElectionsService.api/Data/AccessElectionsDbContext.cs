using Microsoft.EntityFrameworkCore;
using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Data
{
    public class AccessElectionsDbContext : DbContext
    {
        public AccessElectionsDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionSeverity> QuestionSeverities { get; set; }
        public DbSet<QuestionAnswer> Answers { get; set; }
        public DbSet<QuestionAnswerRecommendation> Recommendations { get; set; }
        public DbSet<QuestionAnswerFinding> Findings { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Zone> Zones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccessElectionsDbContext).Assembly);

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
                    new QuestionSeverity() { Id = severity, QuestionSeverityText = severity }
                );
            }

            modelBuilder.Entity<QuestionType>().HasData(
            new QuestionType() { Id = 1, QuestionTypeText = "blank" },
            new QuestionType() { Id = 2, QuestionTypeText = "singleSelection" },
            new QuestionType() { Id = 3, QuestionTypeText = "multipleChoiceSelection" }
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
