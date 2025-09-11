// akna_api.Domain.Entities.Identities/Role.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Sistemdeki kullanıcı rollerini tanımlar (örn: Admin, Shipper, Carrier, Driver).
    /// </summary>
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } // Rol adı (örn: "Admin", "Shipper")

        [StringLength(500)]
        public string Description { get; set; }

        // İlişkiler
        /// <summary>
        /// Bu role sahip kullanıcıların koleksiyonu (Many-to-Many).
        /// </summary>
        public ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Rolün sahip olduğu izinlerin koleksiyonu (Many-to-Many).
        /// </summary>
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}