using TodoApp.Domain.Common;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public class TaskItemEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid UserId { get; set; }

    public Guid? CategoryId { get; set; }

    // Navigation Properties
    public UserEntity User { get; set; } = null!;

    public CategoryEntity? Category { get; set; }
}