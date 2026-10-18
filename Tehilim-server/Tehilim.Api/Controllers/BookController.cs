using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Services;
using Tehilim.API.Extension;

namespace Tehilim.API.Controllers;

[ApiController]
[Route("api/books")]
[Produces("application/json")]
[Authorize]
public class BooksController(IBookService bookService) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>רשימת ספרים פתוחים ופומביים, עם סינון, מיון ודפדוף.</summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<PagedResult<BookListItemDto>>(200), ProducesResponseType(400)]
    public async Task<IActionResult> GetAll([FromQuery] BookQueryParameters query, CancellationToken ct) =>
        Ok(await bookService.GetPublicOpenAsync(query, ct));

    /// <summary>הספרים של המשתמש המחובר, כולל ספרים שאינם פומביים.</summary>
    [HttpGet("mine")]
    [ProducesResponseType<IReadOnlyList<BookListItemDto>>(200), ProducesResponseType(401)]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        Ok(await bookService.GetMyBooksAsync(CurrentUserId, ct));

    /// <summary>ספר בודד לפי מזהה, כולל ספרים שאינם פומביים.</summary>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType<BookDetailsDto>(200), ProducesResponseType(404)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct) =>
        (await bookService.GetByIdAsync(id, ct)).ToActionResult();

    /// <summary>יצירת ספר חדש עם 150 פרקים פנויים.</summary>
    [HttpPost]
    [ProducesResponseType<BookDetailsDto>(201), ProducesResponseType(400), ProducesResponseType(401)]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var result = await bookService.CreateAsync(request, CurrentUserId, ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.ToActionResult();
    }

    /// <summary>עדכון יעד ופומביות של ספר. לבעלים בלבד.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBookRequest request, CancellationToken ct) =>
        (await bookService.UpdateAsync(id, request, CurrentUserId, ct)).ToActionResult();

    /// <summary>מחיקת ספר. לבעלים או למנהל.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        (await bookService.DeleteAsync(id, CurrentUserId, User.IsInRole(nameof(UserRole.Admin)), ct)).ToActionResult();
}