using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Common;

namespace Shared.BuildingBlocks.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkResult()
            : result.Error.ToActionResult();

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(result.Value)
            : result.Error.ToActionResult();

    public static IActionResult ToCreatedResult<T>(this Result<T> result, string routeName, object routeValues) =>
        result.IsSuccess
            ? new CreatedAtRouteResult(routeName, routeValues, result.Value)
            : result.Error.ToActionResult();

    private static IActionResult ToActionResult(this Error error)
    {
        var statusCode = error.Code.Split('.')[^1] switch
        {
            "NotFound"  => StatusCodes.Status404NotFound,
            "Conflict"  => StatusCodes.Status409Conflict,
            "Forbidden" => StatusCodes.Status403Forbidden,
            _           => StatusCodes.Status400BadRequest
        };

        return new ObjectResult(new ProblemDetails
        {
            Title  = error.Code,
            Detail = error.Description,
            Status = statusCode
        })
        { StatusCode = statusCode };
    }
}
