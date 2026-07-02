using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<TaskItemEntity> Tasks { get; set; } = new List<TaskItemEntity>();

    public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
}