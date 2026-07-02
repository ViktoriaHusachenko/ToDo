using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class CategoryEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public Guid UserId { get; set; }

    // Navigation Properties
    public UserEntity User { get; set; } = null!;

    public ICollection<TaskItemEntity> Tasks { get; set; } = new List<TaskItemEntity>();
}