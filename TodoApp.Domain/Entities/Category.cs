using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public Guid UserId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}