using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleBlog.Application.Posts.Commands.CreateComment;
using SimpleBlog.Application.Posts.Commands.CreatePost;
using SimpleBlog.Application.Posts.Common;
using SimpleBlog.Application.Posts.Queries.GetPostById;
using SimpleBlog.Application.Posts.Queries.GetPosts;

namespace SimpleBlog.WebAPI.Endpoints;

internal static class PostsEndpoint
{
    internal static void MapPostsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/posts")
            .WithTags("posts")
            .WithOpenApi()
            .RequireRateLimiting("fixed")
            .DisableAntiforgery();

        group.MapPost(string.Empty, CreatePost)
            .WithName(nameof(CreatePost));

        group.MapGet(string.Empty, GetPosts)
           .WithName(nameof(GetPosts));

        group.MapGet("{id:guid}", GetPostById)
           .WithName(nameof(GetPostById));

        group.MapPost("{id:guid}/comments", CreatePostComment)
           .WithName(nameof(CreatePostComment));
    }

    internal static async Task<Results<Created<Guid>, BadRequest<string>>> CreatePost(CreatePostCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        // NOTE: using the command as the request for simplicity, but it could be replaced with a request dto that could
        // implement specific validations for this endpoint
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Created($"/api/posts/{result.Value}", result.Value)
            : TypedResults.BadRequest(result.Error);
    }

    internal static async Task<Results<Ok<IEnumerable<PostSummaryDto>>, BadRequest<string>>> GetPosts(IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPostsQuery(), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error);
    }

    internal static async Task<Results<Ok<PostDetailsDto>, NotFound<string>>> GetPostById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPostByIdQuery(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.NotFound(result.Error);
    }

    internal static async Task<Results<Created<Guid>, BadRequest<string>>> CreatePostComment(Guid id,
        AddCommentDto request, IMediator mediator, CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(id, request.Content);
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Created($"/api/posts/{id}/comments/{result.Value}", result.Value)
            : TypedResults.BadRequest(result.Error);
    }
}
