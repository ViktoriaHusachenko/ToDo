using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs.Tasks;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid? CategoryId { get; set; }
}