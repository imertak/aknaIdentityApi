using System;
using System.Collections.Generic;
using akna_api.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Sistemdeki genel kullanıcıyı temsil eder. Shipper, Carrier, Driver veya Admin olabilir.
    /// </summary>
    public class User : BaseEntity
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(500)] // Şifre hash'i için yeterli uzunluk
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)] // Aktif, Pasif, Askıda, Onay Bekliyor vb.
        public string AccountStatus { get; set; } = "Pending"; // Varsayılan durum

        [Required]
        [StringLength(50)] // Shipper, Carrier, Driver, Admin
        public string UserType { get; set; } // Kullanıcının birincil tipi

        /// <summary>
        /// E-posta adresinin doğrulanıp doğrulanmadığını gösterir.
        /// </summary>
        public bool IsEmailConfirmed { get; set; } = false;

        /// <summary>
        /// Telefon numarasının doğrulanıp doğrulanmadığını gösterir.
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; set; } = false;

        // İlişkiler
        /// <summary>
        /// Kullanıcının sahip olduğu rollerin koleksiyonu (Many-to-Many).
        /// </summary>
        public ICollection<Role> Roles { get; set; } = new List<Role>();

        /// <summary>
        /// Kullanıcının kimlik doğrulama tokenları (Refresh Tokenlar gibi).
        /// </summary>
        public ICollection<AuthenticationToken> AuthTokens { get; set; } = new List<AuthenticationToken>();

        /// <summary>
        /// Kullanıcının aktif oturumları.
        /// </summary>
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        /// <summary>
        /// Kullanıcının kayıtlı cihaz bilgileri (örn. Push bildirimleri için).
        /// </summary>
        public ICollection<DeviceInfo> Devices { get; set; } = new List<DeviceInfo>();

        /// <summary>
        /// Kullanıcının iki faktörlü kimlik doğrulama ayarları.
        /// </summary>
        public TwoFactorAuthSetting TwoFactorAuthSetting { get; set; }
    }
}