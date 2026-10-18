using Tehilim.Core.Enums;

namespace Tehilim.Core.Entities;


public class Book
{
    public int Id { get; set; }

    // לצורך מה נפתח הספר
    public string Purpose { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public BookStatus Status { get; set; } = BookStatus.Open;

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    public List<Category> Categories { get; set; } = new();
}
