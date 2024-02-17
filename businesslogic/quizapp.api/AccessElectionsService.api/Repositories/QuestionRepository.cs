using Microsoft.EntityFrameworkCore;
using AccessElectionsService.api.Data;
using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AccessElectionsDbContext _dbContext;

        public QuestionRepository(AccessElectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Question>> GetAllQuestions()
        {
            return await _dbContext.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(ans => ans.Recommendations)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(q => q.Findings)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionById(int id)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return await _dbContext.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(ans => ans.Recommendations)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(q => q.Findings)
                .FirstOrDefaultAsync(q => q.Id == id);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<QuestionAnswer> GetAnswersByQuestionId(int id)
        {
           #pragma warning disable CS8603 // Possible null reference return.
            var result =  await _dbContext.Answers
                .Where(a => a.QuestionId == id).ToListAsync();
            return await _dbContext.Answers
                .FirstOrDefaultAsync(a => a.QuestionId == id);
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
