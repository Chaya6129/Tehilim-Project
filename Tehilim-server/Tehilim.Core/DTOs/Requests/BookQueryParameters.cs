using System.ComponentModel.DataAnnotations;
using Tehilim.Core.Enums;

namespace Tehilim.Core.DTOs;

public class BookQueryParameters
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;

    // חיפוש חופשי לפי יעד הספר
    [MaxLength(100)]
    public string? Search { get; set; }

    public BookSortBy SortBy { get; set; } = BookSortBy.CreatedAt;

    public bool Descending { get; set; } = true;
    public int? CategoryId { get; set; }
}