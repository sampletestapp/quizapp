using AutoMapper;
using AccessElectionsService.api.Models;
using AccessElectionsService.api.Repositories;

namespace AccessElectionsService.api.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestions()
        {
            var questions = await _questionRepository.GetAllQuestions();
            return MapQuestionsToDto(questions);
        }

        public async Task<QuestionDto> GetQuestionById(int id)
        {
            var question = await _questionRepository.GetQuestionById(id);
            return MapQuestionToDto(question);
        }

        public async Task<QuestionDto> CreateQuestion(QuestionDto questionDto)
        {
            var question = _mapper.Map<Question>(questionDto);

            await _questionRepository.CreateQuestion(question);

            return MapQuestionToDto(question);
        }

        public async Task UpdateQuestion(QuestionDto questionDto)
        {
            var existingQuestion = await _questionRepository.GetQuestionById(questionDto.Id);

            if (existingQuestion == null)
            {
                throw new ApplicationException("Question not found.");
            }

            // Update properties of existingQuestion with values from questionDto
            existingQuestion.QuestionText = questionDto.QuestionText;
            existingQuestion.QuestionTypeId = questionDto.QuestionTypeId;

            await _questionRepository.UpdateQuestion(existingQuestion);
        }

        public async Task DeleteQuestion(int id)
        {
            var question = await _questionRepository.GetQuestionById(id);

            if (question == null)
            {
                throw new ApplicationException("Question not found.");
            }

            await _questionRepository.DeleteQuestion(question);
        }

        private List<QuestionDto> MapQuestionsToDto(IEnumerable<Question> questions)
        {
            return questions.Select(q => MapQuestionToDto(q)).ToList();
        }

        private QuestionDto MapQuestionToDto(Question question)
        {
            return _mapper.Map<QuestionDto>(question);
        }

        private Question MapDtoToQuestion(QuestionDto questionDto)
        {
            return _mapper.Map<Question>(questionDto);
        }
    }
}
