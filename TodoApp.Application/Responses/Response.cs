using System.Net;

namespace TodoApp.Application.Responses;

public class Response
{
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

    public static Response Ok(HttpStatusCode statusCode = HttpStatusCode.OK) => new() { StatusCode = statusCode };
    public static Response Error(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorMessage = errorMessage,
        StatusCode = statusCode
    };
}

public sealed class Response<T> : Response
{
    public T Value { get; set; }

    public static Response<T> Ok(T value, HttpStatusCode statusCode = HttpStatusCode.OK) => new()
    {
        Value = value,
        StatusCode = statusCode
    };

    public static Response<T> Error(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorMessage = errorMessage,
        StatusCode = statusCode
    };
}
