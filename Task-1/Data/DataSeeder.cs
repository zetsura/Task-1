using System.Collections.ObjectModel;
using Task_1.Models;

namespace Task_1.Data
{
    public static class DataSeeder
    {
        public static Collection<TodoItem> Seed()
        {
            return new Collection<TodoItem>
                        {
                            new TodoItem
                            {
                                Id = 1,
                                Title = "Buy groceries",
                                Description = "Milk, Bread, Cheese, Eggs",
                                IsCompleted = false,
                            },
                            new TodoItem
                            {
                                Id = 2,
                                Title = "Clean the house",
                                Description = "Living room, Kitchen, Bathroom",
                                IsCompleted = false,
                            },
                            new TodoItem
                            {
                                Id = 3,
                                Title = "Finish project report",
                                Description = "Complete the final draft",
                                IsCompleted = false,
                            },
                            new TodoItem
                            {
                                Id = 4,
                                Title = "Call mom",
                                Description = "Weekly check-in",
                                IsCompleted = false,
                            },
                            new TodoItem
                            {
                                Id = 5,
                                Title = "Pay bills",
                                Description = "Electricity, Internet, Water",
                                IsCompleted = false,
                            }
                        };
        }
    }
}
