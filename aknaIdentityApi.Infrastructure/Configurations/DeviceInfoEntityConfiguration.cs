using aknaIdentityApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace aknaIdentityApi.Infrastructure.Configurations
{
    public class DeviceInfoConfiguration : IEntityTypeConfiguration<DeviceInfo>
    {
        public void Configure(EntityTypeBuilder<DeviceInfo> builder)
        {

            builder.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DeviceType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.DeviceModel)
                .HasMaxLength(100);

            builder.Property(x => x.IPAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PushToken)
                .HasMaxLength(500);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}
