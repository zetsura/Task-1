using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Task_1.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; }
    }
}
