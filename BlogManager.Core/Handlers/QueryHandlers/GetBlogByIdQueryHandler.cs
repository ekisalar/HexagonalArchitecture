using BlogManager.Core.DTOs;
using BlogManager.Core.Queries;
using Mapster;
using MediatR;

namespace BlogManager.Core.Handlers.QueryHandlers;

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, BlogDto?>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogByIdQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }


    public async Task<BlogDto?> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.GetBlogByIdAsync(request.Id);
        return blog?.Adapt<BlogDto>();
    }
}