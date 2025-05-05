using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_1.Models;
using Task_1.Repository;
using Xunit;

namespace Task_1.Tests.Repository
{
    public class TodoRepositoryTests
    {
        private static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoDatabase_" + System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new TodoRepository(context);

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test1", Description = "Description1", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Test2", Description = "Description2", IsCompleted = true }
            };

            await context.TodoItems.AddRangeAsync(todoItems);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, item => item.Id == 1);
            Assert.Contains(result, item => item.Id == 2);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectItem()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new TodoRepository(context);

            var todoItem = new TodoItem { Id = 1, Title = "Test", Description = "Description", IsCompleted = false };
            await context.TodoItems.AddAsync(todoItem);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task AddAsync_AddsItemToDatabase()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new TodoRepository(context);

            var todoItem = new TodoItem { Title = "New Task", Description = "New Description", IsCompleted = false };

            // Act
            await repository.AddAsync(todoItem);

            // Assert
            var result = await context.TodoItems.FirstOrDefaultAsync(t => t.Title == "New Task");
            Assert.NotNull(result);
            Assert.Equal("New Description", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingItem()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new TodoRepository(context);

            var todoItem = new TodoItem { Id = 1, Title = "Test", Description = "Description", IsCompleted = false };
            await context.TodoItems.AddAsync(todoItem);
            await context.SaveChangesAsync();

            // Act
            todoItem.Title = "Updated Title";
            todoItem.Description = "Updated Description";
            todoItem.IsCompleted = true;
            await repository.UpdateAsync(todoItem);

            // Assert
            var updated = await context.TodoItems.FindAsync(1);
            Assert.Equal("Updated Title", updated!.Title);
            Assert.Equal("Updated Description", updated.Description);
            Assert.True(updated.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_RemovesItemFromDatabase()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new TodoRepository(context);

            var todoItem = new TodoItem { Id = 1, Title = "Test", Description = "Description", IsCompleted = false };
            await context.TodoItems.AddAsync(todoItem);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(1);

            // Assert
            var result = await context.TodoItems.FindAsync(1);
            Assert.Null(result);
        }
    }
}
