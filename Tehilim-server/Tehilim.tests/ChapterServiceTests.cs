using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Exceptions;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Results;
using Tehilim.Service.Services;

namespace Tehilim.Tests;

public class ChapterServiceTests
{
    private const int BookId = 1;
    private const int ChapterNumber = 5;

    private readonly Mock<IChapterRepository> _chapters = new();
    private readonly Mock<IBookRepository> _books = new();
    private readonly ChapterService _sut;

    public ChapterServiceTests()
    {
        _sut = new ChapterService(
            _chapters.Object,
            _books.Object,
            Mock.Of<IMapper>(),
            NullLogger<ChapterService>.Instance);
    }

    private Chapter SetupChapter(
        ChapterStatus chapterStatus = ChapterStatus.Available,
        BookStatus bookStatus = BookStatus.Open)
    {
        var chapter = new Chapter
        {
            BookId = BookId,
            Number = ChapterNumber,
            Status = chapterStatus,
            Book = new Book { Id = BookId, Status = bookStatus }
        };

        _chapters
            .Setup(r => r.GetForUpdateAsync(BookId, ChapterNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chapter);

        return chapter;
    }

    private Task<Result<ChapterDto>> TakeAsGuestAsync() =>
        _sut.TakeAsync(BookId, ChapterNumber, userId: null,
            new TakeChapterRequest { GuestName = "Guest" }, CancellationToken.None);

    [Fact]
    public async Task TakeAsync_AvailableChapterInOpenBook_SucceedsAndMarksChapterTaken()
    {
        // Arrange
        var chapter = SetupChapter();

        // Act
        var result = await TakeAsGuestAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ChapterStatus.Taken, chapter.Status);
        Assert.Equal("Guest", chapter.TakenByName);
        _chapters.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task TakeAsync_ChapterAlreadyTaken_ReturnsConflictAndDoesNotSave()
    {
        SetupChapter(chapterStatus: ChapterStatus.Taken);

        var result = await TakeAsGuestAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error);
        _chapters.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TakeAsync_SaveThrowsConcurrencyConflict_ReturnsConflictAndDoesNotCompleteBook()
    {
        SetupChapter();
        _chapters
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
.ThrowsAsync(new ConcurrencyConflictException("test"));
        var result = await TakeAsGuestAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error);
        _books.Verify(r => r.CompleteIfNoAvailableChaptersAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TakeAsync_ChapterDoesNotExist_ReturnsNotFound()
    {
        // ללא Setup: ה-Repository המדומה מחזיר null כברירת מחדל

        var result = await TakeAsGuestAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error);
    }

    [Fact]
    public async Task TakeAsync_SuccessfulTake_TriggersBookCompletionCheckWithoutCancellation()
    {
        SetupChapter();

        await TakeAsGuestAsync();

        _books.Verify(r => r.CompleteIfNoAvailableChaptersAsync(BookId, CancellationToken.None), Times.Once);
    }
}