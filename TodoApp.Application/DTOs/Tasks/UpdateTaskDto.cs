using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs.Tasks;

public class UpdateTaskDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid? CategoryId { get; set; }
}