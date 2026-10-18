using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Results;

namespace Tehilim.Core.Interfaces.Services
{
    public interface IBookService
    {
        Task<PagedResult<BookListItemDto>> GetPublicOpenAsync(BookQueryParameters query, CancellationToken ct);
        Task<IReadOnlyList<BookListItemDto>> GetMyBooksAsync(int userId, CancellationToken ct);
        Task<Result<BookDetailsDto>> GetByIdAsync(int id, CancellationToken ct);                         // 404
        Task<Result<BookDetailsDto>> CreateAsync(CreateBookRequest request, int ownerId, CancellationToken ct);
        Task<Result> UpdateAsync(int id, UpdateBookRequest request, int userId, CancellationToken ct);   // 404, 403
        Task<Result> DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct);                // 404, 403
    }
}
