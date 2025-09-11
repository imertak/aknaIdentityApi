using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Kullanıcının uygulamaya eriştiği cihaz bilgilerini saklar (örn: Push Notification token).
    /// </summary>
    public class DeviceInfo : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Cihazın benzersiz kimliği.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Bildirimler için kullanılan token (örn: Firebase Cloud Messaging token).
        /// </summary>
        [StringLength(500)]
        public string PushNotificationToken { get; set; }

        /// <summary>
        /// Cihazın işletim sistemi (örn: iOS, Android, Web).
        /// </summary>
        [StringLength(50)]
        public string OsType { get; set; }

        /// <summary>
        /// Cihazın modeli (örn: iPhone 15, Samsung Galaxy S24).
        /// </summary>
        [StringLength(100)]
        public string DeviceModel { get; set; }

        /// <summary>
        /// Bu cihaz bilgisinin aktif olup olmadığını gösterir.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // İlişkiler
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
