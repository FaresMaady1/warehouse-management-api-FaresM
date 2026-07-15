namespace WebApi.Middleware;

using Microsoft.AspNetCore.Http;
using Warehouse.Domain.Exceptions;

public static class ExceptionStatusMapper
{
    public static (int StatusCode, string ErrorCode) Map(Exception exception) => exception switch
    {
        NotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND"),
        DomainException => (StatusCodes.Status400BadRequest, "BUSINESS_RULE_VIOLATION"),
        _ => (StatusCodes.Status500InternalServerError, "UNEXPECTED_ERROR")
    };
}