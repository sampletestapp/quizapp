using Microsoft.EntityFrameworkCore;
using quizapp.api.Data;
using quizapp.api.Models;

namespace quizapp.api.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuizDbContext _dbContext;

        public QuestionRepository(QuizDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Question>> GetAllQuestions()
        {
            return await _dbContext.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.Answers)
                    .ThenInclude(ans => ans.Recommendations)
                .Include(q => q.Answers)
                    .ThenInclude(q => q.Findings)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionById(int id)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return await _dbContext.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.Answers)
                    .ThenInclude(ans => ans.Recommendations)
                .Include(q => q.Answers)
                    .ThenInclude(q => q.Findings)
                .FirstOrDefaultAsync(q => q.Id == id);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task CreateQuestion(Question question)
        {
            await _dbContext.Questions.AddAsync(question);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateQuestion(Question question)
        {
            _dbContext.Entry(question).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteQuestion(Question question)
        {
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync();
        }
    }
}
