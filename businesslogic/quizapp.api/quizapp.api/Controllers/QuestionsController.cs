using Microsoft.AspNetCore.Mvc;
using quizapp.api.Models;
using quizapp.api.Services;

namespace quizapp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // GET: api/questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        {
            var questions = await _questionService.GetQuestions();
            return Ok(questions);
        }

        // GET: api/questions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionById(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // POST: api/questions
        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(QuestionDto questionDto)
        {
            var createdQuestion = await _questionService.CreateQuestion(questionDto);

            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
        }

        // PUT: api/questions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, QuestionDto questionDto)
        {
            if (id != questionDto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _questionService.UpdateQuestion(questionDto);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionService.GetQuestionById(id);

            if (question == null)
            {
                return NotFound();
            }

            await _questionService.DeleteQuestion(id);

            return NoContent();
        }
    }
}
