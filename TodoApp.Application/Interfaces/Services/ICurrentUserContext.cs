namespace TodoApp.Application.Interfaces.Services;

public interface ICurrentUserContext
{
    Guid? Id { get; }

    string? Email { get; }

    string? Name { get; }

    bool IsAuthenticated { get; }
}
