using aknaIdentityApi.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("Companies")]
    public class Company : BaseEntity
    {
        public long AdminUserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; } = "TR";
        public bool UseEArsiv { get; set; }
        public bool UseEFatura { get; set; }
        public string TaxNumber { get; set; }
        public string MersisNo { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }
}
