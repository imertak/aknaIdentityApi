using akna_api.Domain.Entities.Base;


namespace akna_api.Domain.Entities.Transporter
{
    /// <summary>
    /// Yük taşıma hizmeti veren şirket veya bireysel taşıyıcı.
    /// </summary>
    public class Carrier : BaseEntity
    {
        /// <summary>
        /// Bu taşıyıcıyı temsil eden ana kullanıcının ID'si.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Taşıyıcının şirket adı (bireysel ise kendi adı olabilir).
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Taşıyıcının sorumlu iletişim kişisi.
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// Taşıyıcının vergi numarası.
        /// </summary>
        public string TaxId { get; set; }

        /// <summary>
        /// Taşıyıcının adresi.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Taşıyıcının bulunduğu şehir.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Taşıyıcının bulunduğu eyalet/il.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Taşıyıcının posta kodu.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Taşıyıcının bulunduğu ülke.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Taşıyıcının sistemdeki durumu (örn: Onaylandı, Beklemede, Reddedildi).
        /// </summary>
        public string CarrierStatus { get; set; }

        /// <summary>
        /// Bu taşıyıcıya ait araçların koleksiyonu.
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; }

        /// <summary>
        /// Bu taşıyıcıya ait sürücülerin koleksiyonu.
        /// </summary>
        public ICollection<Driver> Drivers { get; set; }
    }
}
