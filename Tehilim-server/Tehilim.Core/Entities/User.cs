using Tehilim.Core.Enums;

namespace Tehilim.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Book> OwnedBooks { get; set; } = new List<Book>();
    public List<Chapter> TakenChapters { get; set; } = new();
}
