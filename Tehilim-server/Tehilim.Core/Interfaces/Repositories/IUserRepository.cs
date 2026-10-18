using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.Entities;

namespace Tehilim.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);   // התחברות
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);   // הרשמה → 409
        Task AddAsync(User user, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
