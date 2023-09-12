using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Domain;
using BlogManager.Core.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Adapter.PostgreSQL.Tests;

[TestFixture]
public class AuthorRepositoryTests
{
    private IBlogDbContext dbContext;

    [SetUp]
    public async Task Setup()
    {
        dbContext = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
    }

    [Test]
    public async Task GetAuthorByIdAsync_ReturnsAuthorIfExists()
    {
        var authorId         = dbContext.Authors.First().Id;
        var authorRepository = new AuthorRepository(dbContext);

        var result = await authorRepository.GetAuthorByIdAsync(authorId);

        result.Should().NotBeNull();
        result.Id.Should().Be(authorId);
    }

    [Test]
    public async Task GetAuthorByIdAsync_ReturnsNullForNonExistentAuthor()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var authorRepository = new AuthorRepository(dbContext);

        // Act
        var result = await authorRepository.GetAuthorByIdAsync(authorId);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllAuthorsAsync_ReturnsListOfAuthors()
    {
        // Arrange
        var authorRepository = new AuthorRepository(dbContext);

        // Act
        var result = await authorRepository.GetAllAuthorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Author>>();
    }

    [Test]
    public async Task AddAuthorAsync_AddsAuthorToDatabase()
    {
        // Arrange
        var author           = await Author.CreateAsync("TestName 1", "TestSurname 1");
        var authorRepository = new AuthorRepository(dbContext);

        // Act
        var result = await authorRepository.AddAuthorAsync(author);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        // Check if the author is actually added to the database
        var addedAuthor = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == result.Id);
        addedAuthor.Should().NotBeNull();
        addedAuthor.Name.Should().Be(author.Name);
    }

    [Test]
    public async Task UpdateAsync_UpdatesAuthorInDatabase()
    {
        // Arrange
        var author           = dbContext.Authors.First();
        var authorRepository = new AuthorRepository(dbContext);

        // Modify the author
        Author.UpdateAsync(author, "UpdatedAuthor", "UpdatedSurname");
        
        // Act
        var updatedAuthor = await authorRepository.UpdateAsync(author);

        // Assert
        updatedAuthor.Should().NotBeNull();
        updatedAuthor.Name.Should().Be("UpdatedAuthor");

        // Check if the author is updated in the database
        var dbAuthor = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
        dbAuthor.Should().NotBeNull();
        dbAuthor.Name.Should().Be("UpdatedAuthor");
    }

    [Test]
    public async Task DeleteAuthorAsync_DeletesAuthorFromDatabase()
    {
        // Arrange
        var authorToDelete   = dbContext.Authors.First();
        var authorRepository = new AuthorRepository(dbContext);

        // Act
        await authorRepository.DeleteAuthorAsync(authorToDelete);

        // Assert
        // Check if the author is deleted from the database
        var dbAuthor = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorToDelete.Id);
        dbAuthor.Should().BeNull();
    }
}