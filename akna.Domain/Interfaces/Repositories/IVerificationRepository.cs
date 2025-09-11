using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IVerificationRepository : IRepository<Verification>
    {
        Task<Verification> GetByCodeAsync(string code);
        Task<List<Verification>> GetUserVerificationsAsync(int userId, string type);
        Task<Verification> GetLatestUserVerificationAsync(int userId, string type);
        Task ExpireVerificationsAsync(int userId, string type);
        Task<bool> IsCodeValidAsync(string code);
        Task<List<Verification>> GetExpiredVerificationsAsync();
        Task CleanupExpiredVerificationsAsync();
        Task<int> GetRecentVerificationCountAsync(int userId, string type, TimeSpan timeSpan);
    }
}
