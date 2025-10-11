using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace aknaIdentityApi.Infrastructure.Contexts
{
    /// <summary>
    /// Main database context for Akna Identity API
    /// </summary>
    public class AknaIdentityDbContext : DbContext
    {
        public AknaIdentityDbContext(DbContextOptions<AknaIdentityDbContext> options)
            : base(options)
        {
        }

        // DbSet definitions for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DeviceInfo> DeviceInfos { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Configures the database model with entity configurations
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BaseEntity'yi ignore et - ÖNEMLİ!
            modelBuilder.Ignore<BaseEntity>();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Overrides SaveChanges to implement audit fields
        /// </summary>
        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        /// <summary>
        /// Overrides SaveChangesAsync to implement audit fields
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Handles automatic setting of audit fields
        /// </summary>
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && e.Entity.GetType() != typeof(BaseEntity) &&
                    (e.State == EntityState.Added ||
                     e.State == EntityState.Modified ||
                     e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is not BaseEntity entity) continue;

                var now = DateTime.UtcNow;

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        entity.CreatedDate = now;
                        entity.UpdatedDate = now;
                        entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entity.UpdatedDate = now;
                        break;

                    case EntityState.Deleted:
                        entityEntry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.UpdatedDate = now;
                        break;
                }
            }
        }
    }
}