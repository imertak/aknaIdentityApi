

namespace aknaIdentityApi.Domain.Enums
{
    public enum VehicleStatus
    {
        Active = 1,          // Aktif, görev almaya hazır
        InMaintenance = 2,   // Bakımda
        Inactive = 3,        // Pasif / Kullanım dışı
        Decommissioned = 4,  // Hurdaya ayrılmış / Filodan çıkarılmış
        OnDuty = 5           // İşte / Görevde (yük taşıyor)
    }
}
