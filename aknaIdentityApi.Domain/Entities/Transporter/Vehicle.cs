using akna_api.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akna_api.Domain.Entities.Transporter
{
    /// <summary>
    /// Taşıma işlemlerinde kullanılan araç.
    /// </summary>
    public class Vehicle : BaseEntity
    {
        /// <summary>
        /// Aracın ait olduğu taşıyıcının ID'si.
        /// </summary>
        public int CarrierId { get; set; }

        /// <summary>
        /// Aracın plaka numarası.
        /// </summary>
        public string LicensePlate { get; set; }

        /// <summary>
        /// Aracın markası (örn: Mercedes, Ford).
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Aracın modeli (örn: Actros, Transit).
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Aracın üretim yılı.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Aracın tipi (örn: Kamyon, Tır, Kamyonet, Dorse).
        /// </summary>
        public string VehicleType { get; set; }

        /// <summary>
        /// Aracın taşıyabileceği maksimum ağırlık kapasitesi (kilogram cinsinden).
        /// </summary>
        public decimal CapacityKg { get; set; }

        /// <summary>
        /// Aracın taşıyabileceği maksimum hacim kapasitesi (metreküp cinsinden).
        /// </summary>
        public decimal CapacityM3 { get; set; }

        /// <summary>
        /// Aracın mevcut durumu (örn: Müsait, Yolda, Bakımda).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Aracın son bakımının yapıldığı tarih.
        /// </summary>
        public DateTime LastMaintenanceDate { get; set; }

        /// <summary>
        /// Aracın bir sonraki bakımının yapılması gereken tarih.
        /// </summary>
        public DateTime NextMaintenanceDate { get; set; }

        /// <summary>
        /// Aracın şasi numarası.
        /// </summary>
        public string VinNumber { get; set; }

        /// <summary>
        /// Aracın kullandığı yakıt tipi (örn: Dizel, Benzin).
        /// </summary>
        public string FuelType { get; set; }
    }
}
