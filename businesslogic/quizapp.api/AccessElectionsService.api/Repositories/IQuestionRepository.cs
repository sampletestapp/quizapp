using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Repositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllQuestions();
        Task<Question> GetQuestionById(int id);
        Task<QuestionAnswer> GetAnswersByQuestionId(int id);
        Task CreateQuestion(Question question);
        Task UpdateQuestion(Question question);
        Task DeleteQuestion(Question question);
    }
}
