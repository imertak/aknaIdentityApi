

using aknaIdentityApi.Domain.Base;
using aknaIdentityApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("Vehicles")]
    public class Vehicle : BaseEntity
    {
        public long CompanyId { get; set; }                       // Hangi şirkete ait
        public long? CurrentDriverId { get; set; }                // Şu anki sürücü (varsa)
        public long DocumentId { get; set; }                      // Araç ruhsatı doküman ID'si

        public string PlateNumber { get; set; } = default!;       // Plaka (örn: 34ABC123)
        public string? ChassisNo { get; set; }                    // Şasi No (alternatif)
        public int? ModelYear { get; set; }
        public string? Make { get; set; }                         // Marka
        public string? Model { get; set; }

        public VehicleType VehicleType { get; set; }              // Tır, Kamyon, Kamyonet...
        public BodyType BodyType { get; set; }                    // Kapalı kasa, Tenteli, Dorse tipi...
        public int AxleCount { get; set; } = 2;                   // Dingil sayısı

        public int? KerbWeight { get; set; }                       // Boş ağırlık, kg
        public int? PayloadCapacity { get; set; }                  // Azami yüklü ağırlık, kg
        public decimal? CargoVolume { get; set; }                 // Kasa içi hacim, m3
        
        // Kasa içi net ölçüler (yük alanı)
        public decimal? CargoInnerLengthM { get; set; }
        public decimal? CargoInnerWidthM { get; set; }
        public decimal? CargoInnerHeightM { get; set; }

        public int? EuroPalletCapacity { get; set; }

        public bool HasLiftgate { get; set; }                     // Arka lift
        public int? LiftgateCapacityKg { get; set; }
        public bool HasCrane { get; set; }                        // Vinç/Hiab
        public bool HasForkliftOnboard { get; set; }
        public bool SideLoadingAvailable { get; set; }            // Yandan yüklem

        public bool IsRefrigerated { get; set; }
        public decimal? TemperatureMinC { get; set; }
        public decimal? TemperatureMaxC { get; set; }

        // ADR / Tehlikeli madde
        public bool HazmatAllowed { get; set; }
        public string? AdrClasses { get; set; }                   // Örn: "2,3,4.1,8"
        public bool HasAdrKit { get; set; }                       // ADR seti mevcut mu
        public bool HasSpillKit { get; set; }

        // Konteyner / ISO uyumluluğu
        public bool CanCarryContainer { get; set; }
        public string? ContainerSizes { get; set; }               // "20ft,40ft,45ft"
        public bool HasTwistLocks { get; set; }

        // Operasyonel durum
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;


        // Konum (opsiyonel – son bilinen)
        public decimal? LastKnownLat { get; set; }
        public decimal? LastKnownLng { get; set; }
        public DateTime? LastLocationAt { get; set; }

        // Sigorta & muayene & bakım
        public string? InsurancePolicyNo { get; set; }
        public DateTime? InsuranceValidUntil { get; set; }
        public DateTime? InspectionValidUntil { get; set; }       // TÜVTÜRK muayene
        public DateTime? TachographValidUntil { get; set; }
        public DateTime? AdrInspectionValidUntil { get; set; }
        public DateTime? NextServiceAt { get; set; }
        public int? ServiceIntervalKm { get; set; }


    }
}
