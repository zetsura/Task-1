using Microsoft.EntityFrameworkCore;
using Task_1.Models;

namespace Task_1.Repository
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _context.TodoItems.AsNoTracking().ToListAsync();
        }

        public async Task<TodoItem> GetByIdAsync(int id)
        {
            return await _context.TodoItems.FirstOrDefaultAsync(item => item.Id == id)
                ?? throw new ArgumentException($"TodoItem with ID {id} not found.");
        }

        public async Task AddAsync(TodoItem item)
        {
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem item)
        {
            var existing = await _context.TodoItems.FindAsync(item.Id);
            if (existing != null)
            {
                existing.Title = item.Title;
                existing.Description = item.Description;
                existing.IsCompleted = item.IsCompleted;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
