using Microsoft.AspNetCore.Mvc;
using Moq;
using Task_1.Controllers;
using Task_1.Dtos;
using Task_1.Models;
using Task_1.Repository;
using Xunit;

namespace Task_1.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _controller = new TodoController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test1", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Test2", IsCompleted = true }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoItemDto>>(okResult.Value);
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test", IsCompleted = false };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var item = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(1, item.Id);
        }

        [Fact]
        public async Task GetById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null!);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_WithValidItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new TodoItemDto { Title = "Test", IsCompleted = false };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>()))
                .Callback<TodoItem>(item => item.Id = 42)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnDto = Assert.IsType<TodoItemDto>(createdResult.Value);
            Assert.Equal(42, returnDto.Id);
            Assert.Equal("Test", returnDto.Title);
        }

        [Fact]
        public async Task Update_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var existing = new TodoItem { Id = 1, Title = "Old", IsCompleted = false };
            var dto = new TodoItemDto { Title = "Updated", IsCompleted = true };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(repo => repo.UpdateAsync(existing)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", existing.Title);
            Assert.True(existing.IsCompleted);
        }

        [Fact]
        public async Task Update_WithNonExistentItem_ReturnsNotFound()
        {
            // Arrange
            var dto = new TodoItemDto { Title = "Updated", IsCompleted = true };
            _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null!);

            // Act
            var result = await _controller.Update(999, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var existing = new TodoItem { Id = 1, Title = "ToDelete", IsCompleted = false };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null!);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
