using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_1.Data;
using Task_1.Models;

namespace Task_1.Configuration
{
    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        private const int TitleMaxLength = 100;
        private const int DescriptionMaxLength = 1500;

        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("TodoItems");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder.Property(t => t.Description)
                .HasMaxLength(DescriptionMaxLength);

            builder.HasData(DataSeeder.Seed());
        }
    }
}
