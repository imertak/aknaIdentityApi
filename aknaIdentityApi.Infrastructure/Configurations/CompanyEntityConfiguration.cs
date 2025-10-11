using aknaIdentityApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace aknaIdentityApi.Infrastructure.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.City)
                .HasMaxLength(100);

            builder.Property(x => x.Country)
                .HasMaxLength(50);

            builder.Property(x => x.TaxNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.TaxNumber)
                .IsUnique();

            builder.Property(x => x.MersisNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.Website)
                .HasMaxLength(200);
        }
    }
}
