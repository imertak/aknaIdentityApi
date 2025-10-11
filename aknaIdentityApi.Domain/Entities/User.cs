using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("Users")]
    public class User : BaseEntity
    {
        public long CompanyId { get; set; }
        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int TurkishRepublicIdNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public GenderType Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public string ProfileImageUrl { get; set; }
        public UserType UserType { get; set; } 
        public List<int> PermissionIds { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
    }
}
