using Microsoft.EntityFrameworkCore;
using Tehilim.Core.Enums;

namespace Tehilim.Tests;

public class ChapterConcurrencyTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    private const int BookId = 1;
    private const int ChapterNumber = 1;

    [Fact]
    public async Task SaveChanges_TwoContextsTakeSameChapter_SecondSaveThrowsConcurrencyException()
    {
        // Arrange: שני DbContext נפרדים, כמו שתי בקשות במקביל, קוראים את אותו פרק
        await using var first = fixture.CreateContext();
        await using var second = fixture.CreateContext();

        var chapterInFirst = await first.Chapters.SingleAsync(c => c.BookId == BookId && c.Number == ChapterNumber);
        var chapterInSecond = await second.Chapters.SingleAsync(c => c.BookId == BookId && c.Number == ChapterNumber);

        // Act: שניהם משנים; הראשון שומר בהצלחה
        chapterInFirst.Status = ChapterStatus.Taken;
        chapterInFirst.TakenByName = "First";
        await first.SaveChangesAsync();

        chapterInSecond.Status = ChapterStatus.Taken;
        chapterInSecond.TakenByName = "Second";

        // Assert: השמירה של השני נדחית, והנתון של הראשון נשמר
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());

        await using var verify = fixture.CreateContext();
        var saved = await verify.Chapters.SingleAsync(c => c.BookId == BookId && c.Number == ChapterNumber);
        Assert.Equal("First", saved.TakenByName);
    }
}