using Microsoft.AspNetCore.Mvc;

namespace MergeDayApi.Endpoints;

public static class StandardEndpointExtensions
{
    public static RouteHandlerBuilder MapStandardPost<TRequest, TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapPost(pattern, handler)
            .Produces<TResponse>(StatusCodes.Status200OK)
            .Produces<TResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .Accepts<TRequest>("application/json");

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardGet<TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapGet(pattern, handler)
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapFileGet(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        string contentType,
        Action<RouteHandlerBuilder>? configureEndpoint = null
    )
    {
        var routeHandler = builder
            .MapGet(pattern, handler)
            .Produces(StatusCodes.Status200OK, typeof(FileResult), contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardPut<TRequest>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapPut(pattern, handler)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .Accepts<TRequest>("application/json");

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardPut<TRequest, TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null
    )
    {
        var route = builder
            .MapPut(pattern, handler)
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .Accepts<TRequest>("application/json");

        configureEndpoint?.Invoke(route);
        return route;
    }

    public static RouteHandlerBuilder MapStandardDelete(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapDelete(pattern, handler)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }
}
