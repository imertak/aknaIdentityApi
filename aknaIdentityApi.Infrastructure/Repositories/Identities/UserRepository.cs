using akna_api.Infrastructure.Repositories.Base;
using akna_api.Domain.Interfaces.Repositories.Identities;
using aknaIdentities_api.Domain.Entities;
using akna_api.Infrastructure.Context;
using akna_api.Domain.Interfaces.Repositories;

namespace akna_api.Infrastructure.Repositories.Identities
{ 
    /// <summary>
    /// IUserRepository arayüzünün Entity Framework Core implementasyonu.
    /// Kullanıcı verilerine veritabanı erişimini sağlar.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context)
        {
            context = context;
        }

        public async Task<>

    }
}