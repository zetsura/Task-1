using Microsoft.AspNetCore.Mvc;
using Task_1.Dtos;
using Task_1.Models;
using Task_1.Repository;

namespace Task_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll()
        {
            var todoItems = await _repository.GetAllAsync();

            var todoItemDtos = todoItems.Select(todo => new TodoItemDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted
            }).ToList();

            return Ok(todoItemDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoItemDto>> GetById(int id)
        {
            var todoItem = await _repository.GetByIdAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var todoItemDto = new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };

            return Ok(todoItemDto);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> Create([FromBody] TodoItemDto todoItemDto)
        {
            var todoItem = new TodoItem
            {
                Title = todoItemDto.Title,
                Description = todoItemDto.Description,
                IsCompleted = todoItemDto.IsCompleted
            };

            await _repository.AddAsync(todoItem);

            var createdDto = new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };

            return CreatedAtAction(nameof(GetById), new { id = todoItem.Id }, createdDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoItemDto todoItemDto)
        {
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Title = todoItemDto.Title;
            existingItem.Description = todoItemDto.Description;
            existingItem.IsCompleted = todoItemDto.IsCompleted;

            await _repository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
