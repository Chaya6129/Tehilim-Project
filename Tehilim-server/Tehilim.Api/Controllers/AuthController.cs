using Microsoft.AspNetCore.Mvc;
using Tehilim.API.Extension;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Interfaces.Services;

namespace Tehilim.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>הרשמת משתמש חדש. מחזיר token, כך שהמשתמש מחובר מיד.</summary>
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(200), ProducesResponseType(400), ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct) =>
        (await authService.RegisterAsync(request, ct)).ToActionResult();

    /// <summary>התחברות עם אימייל וסיסמה. מחזיר token.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(200), ProducesResponseType(400), ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct) =>
        (await authService.LoginAsync(request, ct)).ToActionResult();
}