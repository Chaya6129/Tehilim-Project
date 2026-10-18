using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tehilim.API.Extension;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Services;

namespace Tehilim.API.Controllers;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>רשימת כל הקטגוריות, ממוינת לפי שם.</summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CategoryDto>>(200)]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await categoryService.GetAllAsync(ct));

    /// <summary>יצירת קטגוריה. למנהל בלבד.</summary>
    [HttpPost]
    [ProducesResponseType<CategoryDto>(201), ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken ct)
    {
        var result = await categoryService.CreateAsync(request, ct);
        return result.IsSuccess ? StatusCode(201, result.Value) : result.ToActionResult();
    }

    /// <summary>עדכון שם של קטגוריה. למנהל בלבד.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404), ProducesResponseType(409)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryRequest request, CancellationToken ct) =>
        (await categoryService.UpdateAsync(id, request, ct)).ToActionResult();

    /// <summary>מחיקת קטגוריה. הספרים המשויכים אליה לא נמחקים. למנהל בלבד.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        (await categoryService.DeleteAsync(id, ct)).ToActionResult();
}