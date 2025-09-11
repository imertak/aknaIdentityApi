using akna_api.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akna_api.Domain.Entities.Transporter
{
    /// <summary>
    /// Yük taşıyan sürücü.
    /// </summary>
    public class Driver : BaseEntity
    {
        /// <summary>
        /// Bu sürücüyü temsil eden ana kullanıcının ID'si.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Sürücünün bağlı olduğu taşıyıcının ID'si.
        /// </summary>
        public int CarrierId { get; set; }

        /// <summary>
        /// Sürücünün ehliyet numarası.
        /// </summary>
        public string DriverLicenseNumber { get; set; }

        /// <summary>
        /// Sürücü ehliyetinin son kullanma tarihi.
        /// </summary>
        public DateTime LicenseExpiryDate { get; set; }

        /// <summary>
        /// Sürücünün sistemdeki durumu (örn: Aktif, Pasif, İzinli).
        /// </summary>
        public string DriverStatus { get; set; }

        /// <summary>
        /// Sürücünün şu anda atalı olduğu aracın ID'si (nullable).
        /// </summary>
        public int? CurrentVehicleId { get; set; }

        /// <summary>
        /// Sürücünün yük almaya müsait olup olmadığını gösterir.
        /// </summary>
        public bool IsAvailable { get; set; }
    }

}
