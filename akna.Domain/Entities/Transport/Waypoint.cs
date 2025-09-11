using akna_api.Domain.Entities.Base;

namespace akna_api.Domain.Entities.Transport
{
    /// <summary>
    /// Bir rotadaki ara durak noktası.
    /// </summary>
    public class Waypoint : BaseEntity
    {
        /// <summary>
        /// Durağın ilişkili olduğu rotanın ID'si.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Durağın konumunun ID'si.
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        /// Durakların sırası.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Durağa planlanan varış zamanı (nullable).
        /// </summary>
        public DateTime? ScheduledArrivalTime { get; set; }

        /// <summary>
        /// Durağa gerçek varış zamanı (nullable).
        /// </summary>
        public DateTime? ActualArrivalTime { get; set; }

        /// <summary>
        /// Duraktan planlanan ayrılış zamanı (nullable).
        /// </summary>
        public DateTime? ScheduledDepartureTime { get; set; }

        /// <summary>
        /// Duraktan gerçek ayrılış zamanı (nullable).
        /// </summary>
        public DateTime? ActualDepartureTime { get; set; }

        /// <summary>
        /// Durakla ilgili ek notlar.
        /// </summary>
        public string Notes { get; set; }
    }
}
