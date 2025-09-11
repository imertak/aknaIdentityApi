using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using akna_api.Domain.Entities.Base;

namespace aknaIdentities_api.Domain.Entities
{
    /// <summary>
    /// Kullanıcının oturum açma token'ını (örn: JWT refresh token) temsil eder.
    /// </summary>
    public class AuthenticationToken : BaseEntity
    {
        [Required]
        public int UserId { get; set; } // Hangi kullanıcıya ait olduğu

        /// <summary>
        /// Token'ın kendisi (örn: Refresh Token).
        /// </summary>
        [Required]
        [StringLength(2000)] // JWT veya benzeri token'lar için yeterli uzunluk
        public string Token { get; set; }

        [StringLength(50)]
        public string TokenType { get; set; } // "AccessToken", "RefreshToken", vb.

        /// <summary>
        /// Token'ın son kullanma tarihi (UTC).
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Token'ın iptal edilip edilmediğini gösterir (örn: kullanıcı çıkış yaptığında veya şifre değiştiğinde).
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// Token'ın neden iptal edildiği (örn: 'Logout', 'PasswordChange', 'Compromised').
        /// </summary>
        [StringLength(100)]
        public string RevokedReason { get; set; }

        /// <summary>
        /// Token'ın oluşturulduğu IP adresi.
        /// </summary>
        [StringLength(45)] // IPv6 için yeterli uzunluk
        public string IpAddress { get; set; }

        // İlişkiler
        [ForeignKey("UserId")]
        public User User { get; set; } // İlgili kullanıcı
    }
}