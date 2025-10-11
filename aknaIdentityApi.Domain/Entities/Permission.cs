using aknaIdentityApi.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("Permissions")]
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
