using akna_api.Domain.Entities.Base;


namespace akna_api.Domain.Entities.Transport
{
    /// <summary>
    /// Bir taşıma emrinin (Load) rotasını ve üzerindeki durakları tanımlar.
    /// </summary>
    public class Route : BaseEntity
    {
        /// <summary>
        /// Rotanın ilişkili olduğu yükün ID'si.
        /// </summary>
        public int LoadId { get; set; }

        /// <summary>
        /// Rotanın adı veya tanımı.
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Rotanın toplam tahmini mesafesi (kilometre cinsinden).
        /// </summary>
        public decimal TotalDistanceKm { get; set; }

        /// <summary>
        /// Rotanın tahmini seyahat süresi.
        /// </summary>
        public TimeSpan EstimatedTravelTime { get; set; }

        /// <summary>
        /// Harita üzerinde rotayı çizen kodlanmış çizgi verisi (örn: Google Polyline formatında).
        /// </summary>
        public string RoutePolyline { get; set; }

        /// <summary>
        /// Rota üzerindeki ara durak noktalarının koleksiyonu.
        /// </summary>
        public ICollection<Waypoint> Waypoints { get; set; }
    }
}
