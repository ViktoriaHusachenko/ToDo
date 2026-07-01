using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}