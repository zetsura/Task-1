# Todo API

A RESTful API for managing tasks using ASP.NET Core based on .NET 9 and Entity Framework Core.

## Features
- Full CRUD for tasks (Create, Read, Update, Delete)
- Data validation at the model and API level
- Swagger/OpenAPI documentation with descriptions of all endpoints
- Repository pattern for data access abstraction
- SQL Server support (local database)
- Automatic database population with test data during initialization

## Running the Application

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- IDE (Visual Studio 2022 recommended)

### Configuring the Database Connection
Before running the application, you need to configure the database connection in the `appsettings.json` file:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TodoDb;Trusted_Connection=True;"
  }
}
```

### Running Migration Commands
```bash
Add-Migration Init
Update-Database
```
# Task Feedback

## 1. Was it easy to complete the task using AI?
Yes, there were no significant difficulties. The tasks were completed easily.

## 2. How much time did it take to complete the task?
The task took approximately 30-40 minutes, depending on the complexity.

## 3. Was the code ready to run after generation? What did you have to change to make it usable?
The code was almost ready to use. I just needed to adjust the connection strings and add data validation.

## 4. What difficulties did you encounter while completing the task?
There were no issues. The tasks were completed without any major difficulties.

## 5. What specific tips did you learn as best practices for completing the task?
It is important to phrase promts correctly to get accurate and relevant results from ChatGPT.

# CHAT LOG

`User:` I need a repository interface for the TodoItem model with possible CRUD operations. Each method should be asynchronous and return a Task. Include methods for getting all items, getting by id, adding, updating, and deleting.

`Github Copilot:` 
```csharp
public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem> GetByIdAsync(int id);
    Task AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(int id);
}
```

`User:` I need to implement a task repository TodoRepository based on the ITodoRepository interface. The repository should use Entity Framework Core with DbContext to work with the store. Implement all asynchronous CRUD operations with proper error handling. In the GetAllAsync method, use AsNoTracking to optimize performance.

`Github Copilot:`
```csharp
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
```

`User:` I need a RESTful API controller to work with my TodoItem model. The controller should support basic CRUD operations (get all items, get item by ID, create, update and delete). Please use the Repository pattern to access data via the ITodoRepository interface and add proper error handling.

`Github Copilot:` 
```csharp
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
```

`User:` I need unit tests for TodoRepository using xUnit and Entity Framework Core in-memory database. Implement tests for all repository methods: GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync and DeleteAsync. Each test should create a new context instance with a unique in-memory database name.

`Github Copilot:`
```csharp
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
        Assert.Equal("Updated Title", updated.Title);
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
```

`User:` I need unit tests for TodoController using xUnit and Moq to mock ITodoRepository. Implement tests for all controller methods: GetAll, GetById, Create, Update, and Delete. For each method, write multiple tests that test different scenarios, including success and error handling.

`Github Copilot:` 
```csharp
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
        _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null);

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
        _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null);

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
        _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((TodoItem)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
```
