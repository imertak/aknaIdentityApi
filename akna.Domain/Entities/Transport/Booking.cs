using akna_api.Domain.Entities.Base;


namespace akna_api.Domain.Entities.Transport
{
    /// <summary>
    /// Bir taşıyıcının bir yükü kabul etmesini ve atanmasını temsil eder.
    /// </summary>
    public class Booking : BaseEntity
    {
        /// <summary>
        /// Rezervasyonun ilişkili olduğu yükün ID'si.
        /// </summary>
        public int LoadId { get; set; }

        /// <summary>
        /// Yükü kabul eden taşıyıcının ID'si.
        /// </summary>
        public int CarrierId { get; set; }

        /// <summary>
        /// Bu yüke atanan sürücünün ID'si (nullable).
        /// </summary>
        public int? DriverId { get; set; }

        /// <summary>
        /// Bu yüke atanan aracın ID'si (nullable).
        /// </summary>
        public int? VehicleId { get; set; }

        /// <summary>
        /// Rezervasyonun veya atamanın durumu (örn: Kabul Edildi, Tamamlandı, İptal Edildi).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Rezervasyonun yapıldığı/kabul edildiği tarih ve saat.
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Rezervasyonun veya atamanın onaylandığı tarih ve saat (nullable).
        /// </summary>
        public DateTime? AcceptanceDate { get; set; }

        /// <summary>
        /// Rezervasyonla ilgili ek notlar.
        /// </summary>
        public string Notes { get; set; }
    }

}
