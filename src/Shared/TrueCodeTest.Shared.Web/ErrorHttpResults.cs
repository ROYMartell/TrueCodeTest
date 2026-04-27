using Microsoft.AspNetCore.Http;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Shared.Web;

public static class ErrorHttpResults
{
    public static IResult ToProblem(this Error error) => error.Type switch
    {
        ErrorType.Validation => Results.Problem(
            detail: error.Message,
            title: "Validation failed",
            statusCode: StatusCodes.Status400BadRequest,
            type: $"urn:truecode:error:{error.Code}"),
        ErrorType.NotFound => Results.Problem(
            detail: error.Message,
            title: "Not found",
            statusCode: StatusCodes.Status404NotFound,
            type: $"urn:truecode:error:{error.Code}"),
        ErrorType.Conflict => Results.Problem(
            detail: error.Message,
            title: "Conflict",
            statusCode: StatusCodes.Status409Conflict,
            type: $"urn:truecode:error:{error.Code}"),
        ErrorType.Unauthorized => Results.Problem(
            detail: error.Message,
            title: "Unauthorized",
            statusCode: StatusCodes.Status401Unauthorized,
            type: $"urn:truecode:error:{error.Code}"),
        ErrorType.Forbidden => Results.Problem(
            detail: error.Message,
            title: "Forbidden",
            statusCode: StatusCodes.Status403Forbidden,
            type: $"urn:truecode:error:{error.Code}"),
        _ => Results.Problem(
            detail: error.Message,
            title: "Server error",
            statusCode: StatusCodes.Status500InternalServerError,
            type: $"urn:truecode:error:{error.Code}"),
    };
}
