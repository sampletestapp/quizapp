using quizapp.api.Models;

namespace quizapp.api.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetQuestions();
        Task<QuestionDto> GetQuestionById(int id);
        Task<QuestionDto> CreateQuestion(QuestionDto questionDto);
        Task UpdateQuestion(QuestionDto questionDto);
        Task DeleteQuestion(int id);
    }
}
