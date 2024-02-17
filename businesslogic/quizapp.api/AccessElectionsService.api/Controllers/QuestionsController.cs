using Microsoft.AspNetCore.Mvc;
using AccessElectionsService.api.Models;
using AccessElectionsService.api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AccessElectionsService.api.Controllers
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

        [HttpGet("get-answers")]
        public async Task<ActionResult<QuestionDto>> GetQuestionAnswers(int id)
        {
            var question = await _questionService.GetAnswersByQuestionId(id);

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
            try
            {
                var createdQuestion = await _questionService.CreateQuestion(questionDto);
                return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex, out var errorCode))
            {
                return HandleUniqueConstraintViolation(errorCode, questionDto);
            }
        }
        private static bool IsUniqueConstraintViolation(DbUpdateException ex, out int? errorCode)
        {
            var sqlException = ex.InnerException as SqlException;
            errorCode = sqlException?.Number;
            return errorCode == 2601 || errorCode == 2627;
        }

        // Helper method to handle the unique constraint violation
        private ActionResult HandleUniqueConstraintViolation(int? errorCode, QuestionDto questionDto)
        {
            return errorCode switch
            {
                2601 or 2627 => BadRequest($"Question Number {questionDto.QuestionNumber} Already Exists"),
                _ => throw new InvalidOperationException("Unhandled unique constraint violation.")
            };
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
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex, out var errorCode))
            {
                return HandleUniqueConstraintViolation(errorCode, questionDto);
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
