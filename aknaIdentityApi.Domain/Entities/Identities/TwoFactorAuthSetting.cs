using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Kullanıcının iki faktörlü kimlik doğrulama ayarlarını saklar.
    /// </summary>
    public class TwoFactorAuthSetting : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// İki faktörlü kimlik doğrulamanın etkin olup olmadığı.
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Kullanılan iki faktörlü kimlik doğrulama yöntemi (örn: SMS, Email, Authenticator App).
        /// </summary>
        [StringLength(50)]
        public string Method { get; set; }

        /// <summary>
        /// Authenticator uygulamaları için gizli anahtar (eğer kullanılıyorsa).
        /// </summary>
        [StringLength(200)]
        public string AuthenticatorSecretKey { get; set; }

        // İlişkiler
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}