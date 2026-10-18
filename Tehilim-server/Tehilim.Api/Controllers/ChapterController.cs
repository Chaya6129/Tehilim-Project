using Microsoft.AspNetCore.Mvc;
using Tehilim.API.Extension;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Interfaces.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Tehilim.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/books/{bookId:int}/chapters")]
[Produces("application/json")]
[ProducesResponseType(404)]
public class ChaptersController(IChapterService chapterService) : ControllerBase
{
    private int? CurrentUserId =>
    int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    /// <summary>כל 150 הפרקים של ספר, עם מצב כל פרק.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ChapterDto>>(200)]
    public async Task<IActionResult> GetAll([FromRoute] int bookId, CancellationToken ct) =>
        (await chapterService.GetByBookAsync(bookId, ct)).ToActionResult();

    /// <summary>נטילת פרק. 409 אם משתתף אחר נטל אותו קודם.</summary>
    [HttpPatch("{number:int:range(1,150)}")]
    [ProducesResponseType<ChapterDto>(200), ProducesResponseType(400), ProducesResponseType(409)]
    public async Task<IActionResult> Take(
        [FromRoute] int bookId, [FromRoute] int number,
        [FromBody] TakeChapterRequest dto, CancellationToken ct) =>
        (await chapterService.TakeAsync(bookId, number, userId: null, dto, ct)).ToActionResult();
}