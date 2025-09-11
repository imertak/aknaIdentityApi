using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// E-posta, telefon numarası veya şifre sıfırlama gibi doğrulama işlemlerinde kullanılan geçici kodlar/token'ları saklar.
    /// </summary>
    public class Verification : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Doğrulama kodu veya token'ı.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// Doğrulama işleminin türü (örn: "EmailVerification", "PhoneVerification", "PasswordReset").
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        /// <summary>
        /// Kodun/token'ın son kullanma tarihi (UTC).
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Kodun/token'ın kullanılıp kullanılmadığını gösterir.
        /// </summary>
        public bool IsUsed { get; set; } = false;

        // İlişkiler
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}