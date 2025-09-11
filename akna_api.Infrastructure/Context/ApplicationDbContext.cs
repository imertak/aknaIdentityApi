using aknaIdentities_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace akna_api.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet definitions for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<DeviceInfo> DeviceInfos { get; set; }
        public DbSet<TwoFactorAuthSetting> TwoFactorAuthSettings { get; set; }

    }
}
