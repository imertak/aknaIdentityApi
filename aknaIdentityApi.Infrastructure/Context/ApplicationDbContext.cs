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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User tablosu
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.AccountStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UserType).IsRequired().HasMaxLength(50);
            });

            // Role tablosu
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Permission tablosu
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // AuthenticationToken tablosu
            modelBuilder.Entity<AuthenticationToken>(entity =>
            {
                entity.ToTable("AuthenticationTokens");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.TokenType).HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.RevokedReason).HasMaxLength(100);

                entity.HasOne(t => t.User)
                      .WithMany(u => u.AuthTokens)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Session tablosu
            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Sessions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceInfo).HasMaxLength(200);
                entity.Property(e => e.IpAddress).HasMaxLength(45);

                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sessions)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // DeviceInfo tablosu
            modelBuilder.Entity<DeviceInfo>(entity =>
            {
                entity.ToTable("DeviceInfos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceIdentifier).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PushNotificationToken).HasMaxLength(500);
                entity.Property(e => e.OsType).HasMaxLength(50);
                entity.Property(e => e.DeviceModel).HasMaxLength(100);

                entity.HasOne(d => d.User)
                      .WithMany(u => u.Devices)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TwoFactorAuthSetting tablosu
            modelBuilder.Entity<TwoFactorAuthSetting>(entity =>
            {
                entity.ToTable("TwoFactorAuthSettings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Method).HasMaxLength(50);
                entity.Property(e => e.AuthenticatorSecretKey).HasMaxLength(200);

                entity.HasOne(t => t.User)
                      .WithOne(u => u.TwoFactorAuthSetting)
                      .HasForeignKey<TwoFactorAuthSetting>(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Verification tablosu
            modelBuilder.Entity<Verification>(entity =>
            {
                entity.ToTable("Verifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);

                entity.HasOne(v => v.User)
                      .WithMany()
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

          
        }
    }
}