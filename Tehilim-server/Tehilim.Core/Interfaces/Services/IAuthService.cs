using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Results;

namespace Tehilim.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct);  // 409 אם האימייל קיים
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct);        // 401 בכישלון
    }
}
