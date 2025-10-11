using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace aknaIdentityApi.Domain.Entities
{
    [Table("Verifications")]
    public class Verification : BaseEntity
    {
        public long UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public VerificationType VerificationType { get; set; } 
    }
}
