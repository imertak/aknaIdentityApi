using akna_api.Domain.Entities.Base;


namespace akna_api.Domain.Entities.Transport
{
    /// <summary>
    /// Bir yük sahibinin oluşturduğu taşıma emri veya ilanı.
    /// Taşınacak yükün detaylarını da içerir (Freight entity'si ile birleştirilmiştir).
    /// Sistemdeki ana işlem birimlerinden biridir ve belirlenen fiyatla doğrudan atanır.
    /// </summary>
    public class Load : BaseEntity
    {
        /// <summary>
        /// Yükü oluşturan yük sahibinin ID'si.
        /// </summary>
        public int ShipperId { get; set; }

        /// <summary>
        /// Bu yüke atanan taşıyıcının ID'si (başlangıçta boş olabilir).
        /// </summary>
        public int? CarrierId { get; set; }

        /// <summary>
        /// Yük için sistem tarafından veya kullanıcı tarafından belirlenen benzersiz numara.
        /// </summary>
        public string LoadNumber { get; set; }

        /// <summary>
        /// Yükün mevcut durumu (örn: Yeni, Taşıyıcı Aranıyor, Onaylandı, Yükleniyor, Yolda, Teslim Edildi, İptal Edildi).
        /// </summary>
        public string Status { get; set; }

        // --- YÜK DETAYLARI (eski Freight entity'sinden taşındı) ---
        /// <summary>
        /// Taşınacak yükün genel adı veya tanımı (örn: Paletli Ürünler, İnşaat Malzemesi).
        /// </summary>
        public string FreightName { get; set; }

        /// <summary>
        /// Yükün detaylı açıklaması.
        /// </summary>
        public string FreightDescription { get; set; }

        /// <summary>
        /// Yükün toplam ağırlığı (kilogram cinsinden).
        /// </summary>
        public decimal WeightKg { get; set; }

        /// <summary>
        /// Yükün toplam hacmi (metreküp cinsinden).
        /// </summary>
        public decimal VolumeM3 { get; set; }

        /// <summary>
        /// Taşınacak ürün adedi veya paket sayısı.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Yükün ambalaj tipi (örn: Koli, Palet, Dökme).
        /// </summary>
        public string PackagingType { get; set; }

        /// <summary>
        /// Yükün sınıfı, özellikle Parsiyel Yük (LTL) taşımacılığında kullanılır.
        /// </summary>
        public string FreightClass { get; set; }

        /// <summary>
        /// Yükün tehlikeli madde olup olmadığını gösterir.
        /// </summary>
        public bool IsHazardous { get; set; }

        /// <summary>
        /// Yükün taşınması için özel talimatlar (örn: "Kırılabilir", "Dikkatli Taşınmalı").
        /// </summary>
        public string HandlingInstructions { get; set; }
        // --- YÜK DETAYLARI SONU ---

        /// <summary>
        /// Yükün alınacağı konumun ID'si.
        /// </summary>
        public int PickupLocationId { get; set; }

        /// <summary>
        /// Yükün teslim edileceği konumun ID'si.
        /// </summary>
        public int DeliveryLocationId { get; set; }

        /// <summary>
        /// Yükün alınması gereken tarih ve saat.
        /// </summary>
        public DateTime PickupDate { get; set; }

        /// <summary>
        /// Yükün teslim edilmesi gereken tarih ve saat.
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Yük sahibi tarafından belirlenen, taşıma için ödenmesi gereken toplam fiyat.
        /// </summary>
        public decimal OfferedPrice { get; set; }

        /// <summary>
        /// Ödeme koşulları (örn: "Peşin", "30 gün vadeli").
        /// </summary>
        public string PaymentTerms { get; set; }

        /// <summary>
        /// Yükle ilgili ek notlar veya özel gereksinimler.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Yükün taşınması için gerekli araç tipi (örn: Tır, Kamyon, Kamyonet).
        /// </summary>
        public string RequiredVehicleType { get; set; }

        /// <summary>
        /// Yükün bir tam kamyon yükü (FTL) olup olmadığını gösterir (aksi halde parsiyel yük - LTL).
        /// </summary>
        public bool IsFullTruckLoad { get; set; }

        /// <summary>
        /// Bu yük için yapılan rezervasyonların (taşıyıcı atamalarının) koleksiyonu.
        /// </summary>
        public ICollection<Booking> Bookings { get; set; }
    }
}
