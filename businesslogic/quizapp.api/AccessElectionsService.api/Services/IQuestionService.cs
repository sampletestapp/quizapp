using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetQuestions();
        Task<QuestionDto> GetQuestionById(int id);
        Task<QuestionAnswerDto> GetAnswersByQuestionId(int id);
        Task<QuestionDto> CreateQuestion(QuestionDto questionDto);
        Task UpdateQuestion(QuestionDto questionDto);
        Task DeleteQuestion(int id);
    }
}
