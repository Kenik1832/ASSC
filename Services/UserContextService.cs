using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ASSC.Data;
using Microsoft.EntityFrameworkCore;

namespace ASSC.Services
{
    public class UserContextService
    {
        private readonly IHttpContextAccessor _http;
        private readonly ApplicationDbContext _context;

        public UserContextService(
            IHttpContextAccessor http,
            ApplicationDbContext context)
        {
            _http = http;
            _context = context;
        }

        public string GetUserId()
        {
            return _http.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
        }

        public async Task<int?> GetSupplierId()
        {
            var userId = GetUserId();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.SupplierId;
        }

        public async Task<int?> GetContractorId()
        {
            var userId = GetUserId();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.ContractorId;
        }
    }
}