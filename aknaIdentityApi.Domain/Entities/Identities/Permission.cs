using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Sistemdeki belirli eylemleri veya kaynaklara erişimi temsil eden izinler.
    /// </summary>
    public class Permission : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } // İzin adı (örn: "can_create_load", "can_view_all_users")

        [StringLength(500)]
        public string Description { get; set; }

        // İlişkiler
        /// <summary>
        /// Bu izne sahip rollerin koleksiyonu (Many-to-Many).
        /// </summary>
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}