using System.ComponentModel.DataAnnotations;

namespace Task_1.Dtos
{
    public class TodoItemDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
