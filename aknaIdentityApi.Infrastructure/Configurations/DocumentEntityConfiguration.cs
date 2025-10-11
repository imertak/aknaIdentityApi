using aknaIdentityApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace aknaIdentityApi.Infrastructure.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.DocumentCategory)
                .HasConversion<string>();

            builder.Property(x => x.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasOne<Company>()
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .IsRequired();
        }
    }
}
