using aknaIdentityApi.Domain.Entities;
using aknaIdentityApi.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace aknaIdentityApi.Infrastructure.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {

            builder.Property(x => x.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.PlateNumber)
                .IsUnique();

            builder.Property(x => x.ChassisNo)
                .HasMaxLength(50);

            builder.Property(x => x.Make)
                .HasMaxLength(100);

            builder.Property(x => x.Model)
                .HasMaxLength(100);

            builder.Property(x => x.VehicleType)
                .HasConversion<string>();

            builder.Property(x => x.BodyType)
                .HasConversion<string>();

            builder.Property(x => x.AdrClasses)
                .HasMaxLength(100);

            builder.Property(x => x.ContainerSizes)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasConversion<string>();

            builder.HasOne<Company>()
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CurrentDriverId)
                .IsRequired(false);
        }
    }
}
