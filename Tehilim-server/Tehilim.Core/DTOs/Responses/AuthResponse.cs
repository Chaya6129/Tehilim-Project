using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Responds
{
    public record AuthResponse(string Token, DateTime ExpiresAtUtc, string FullName, string Role);

}
