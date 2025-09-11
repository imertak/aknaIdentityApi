using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Kullanıcının aktif oturumlarını izler.
    /// </summary>
    public class Session : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Oturumun başlangıç zamanı (UTC).
        /// </summary>
        [Required]
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// Oturumun çıkış zamanı (UTC). Null ise oturum hala aktiftir.
        /// </summary>
        public DateTime? LogoutTime { get; set; }

        /// <summary>
        /// Oturumun sona erme zamanı (örn: token süresi dolduğunda).
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Oturum açılan cihazın kimliği veya tipi.
        /// </summary>
        [StringLength(200)]
        public string DeviceInfo { get; set; }

        /// <summary>
        /// Oturum açılan IP adresi.
        /// </summary>
        [StringLength(45)]
        public string IpAddress { get; set; }

        // İlişkiler
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
