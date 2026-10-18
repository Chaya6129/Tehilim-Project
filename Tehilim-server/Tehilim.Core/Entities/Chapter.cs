using Tehilim.Core.Enums;

namespace Tehilim.Core.Entities;

public class Chapter
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    // מספר הפרק, 1 עד 150
    public int Number { get; set; }

    public ChapterStatus Status { get; set; } = ChapterStatus.Available;

    // משתמש רשום שנטל את הפרק; ריק כשאורח נטל אותו
    public int? TakenByUserId { get; set; }
    public User? TakenByUser { get; set; }

    // שם תצוגה של אורח (אופציונלי)
    public string? TakenByName { get; set; }

    public DateTime? TakenAt { get; set; }

    // Concurrency token: מוגדר ב-Fluent API ומוחלף בכל שמירה בדריסת SaveChangesAsync
    public Guid Version { get; set; } = Guid.NewGuid();
}
