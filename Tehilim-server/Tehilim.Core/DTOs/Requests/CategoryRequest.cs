using System.ComponentModel.DataAnnotations;

namespace Tehilim.Core.DTOs.Requests
{
    public class CategoryRequest
    {
        [Required, MaxLength(50)] public string Name { get; set; } = "";
    }
}