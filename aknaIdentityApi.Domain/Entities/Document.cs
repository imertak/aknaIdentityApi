using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("Documents")]
    public class Document : BaseEntity
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public DocumentCategory DocumentCategory { get; set; } // Identity, License, Insurance, etc.
        public string DocumentType { get; set; } // K1, K2, CE, etc.
        public int DocumentNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string FileUrl { get; set; }
        public bool IsVerified { get; set; }
    }
}
