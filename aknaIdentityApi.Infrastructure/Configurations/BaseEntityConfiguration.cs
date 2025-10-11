using aknaIdentityApi.Domain.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace aknaIdentityApi.Infrastructure.Configurations
{
    /// <summary>
    /// Base configuration for all entities inheriting from BaseEntity
    /// </summary>
    public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
    {
        public void Configure(EntityTypeBuilder<BaseEntity> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // CreatedDate configuration
            builder.Property(x => x.CreatedDate)
                .IsRequired();

            // CreatedUser configuration
            builder.Property(x => x.CreatedUser)
                .IsRequired()
                .HasMaxLength(100);

            // UpdatedDate configuration
            builder.Property(x => x.UpdatedDate)
                .IsRequired(true);

            // UpdatedUser configuration
            builder.Property(x => x.UpdatedUser)
                .IsRequired(false)
                .HasMaxLength(100);

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
