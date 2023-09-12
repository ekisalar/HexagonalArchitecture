using BlogManager.Core.Constants;
using BlogManager.Core.DTOs;
using BlogManager.Core.Queries;
using BlogManager.Core.Repositories;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.QueryHandlers;

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, BlogDto?>
{
    private readonly IBlogRepository    _blogRepository;
    private readonly IBlogManagerLogger _logger;

    public GetBlogByIdQueryHandler(IBlogRepository blogRepository, IBlogManagerLogger logger)
    {
        _blogRepository = blogRepository;
        _logger         = logger;
    }


    public async Task<BlogDto?> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.GetBlogByIdAsync(request.Id, request.IncludeAuthorInfo);
        _logger.LogInformation(LoggingConstants.BlogGetSuccessfully);
        return blog?.Adapt<BlogDto>();
    }
}