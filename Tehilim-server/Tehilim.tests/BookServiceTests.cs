using AutoMapper;
using Moq;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Results;
using Tehilim.Service.Services;

namespace Tehilim.Tests;

public class BookServiceTests
{
    private const int BookId = 1;
    private const int OwnerId = 10;
    private const int OtherUserId = 20;

    private readonly Mock<IBookRepository> _books = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _sut = new BookService(_books.Object, _categories.Object, _mapper.Object);
    }

    private Book SetupExistingBook()
    {
        var book = new Book { Id = BookId, OwnerId = OwnerId };
        _books
            .Setup(r => r.GetByIdForUpdateAsync(BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        return book;
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesExactly150AvailableChapters()
    {
        // Arrange
        Book? added = null;
        _mapper.Setup(m => m.Map<Book>(It.IsAny<CreateBookRequest>())).Returns(new Book());
        _books
            .Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .Callback<Book, CancellationToken>((book, _) => added = book);

        // Act
        await _sut.CreateAsync(new CreateBookRequest(), OwnerId, CancellationToken.None);

        // Assert
        Assert.NotNull(added);
        Assert.Equal(150, added.Chapters.Count);
        Assert.All(added.Chapters, c => Assert.Equal(ChapterStatus.Available, c.Status));
        Assert.Equal(Enumerable.Range(1, 150), added.Chapters.Select(c => c.Number));
    }

    [Fact]
    public async Task UpdateAsync_UserIsNotOwner_ReturnsForbiddenAndDoesNotSave()
    {
        SetupExistingBook();

        var result = await _sut.UpdateAsync(BookId, new UpdateBookRequest(), OtherUserId, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.Error);
        _books.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_AdminWhoIsNotOwner_DeletesBook()
    {
        var book = SetupExistingBook();

        var result = await _sut.DeleteAsync(BookId, OtherUserId, isAdmin: true, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _books.Verify(r => r.Remove(book), Times.Once);
        _books.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}