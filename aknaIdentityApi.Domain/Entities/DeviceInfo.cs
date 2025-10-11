using aknaIdentityApi.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace aknaIdentityApi.Domain.Entities
{
    [Table("DeviceInfos")]
    public class DeviceInfo : BaseEntity
    {
        public long UserId { get; set; }
        public string DeviceId { get; set; } = default!;           // UUID/IMEI/Firebase Instance ID
        public string DeviceType { get; set; }                     // iOS, Android, Web
        public string? DeviceModel { get; set; }                  // iPhone 12, Samsung Galaxy S21, etc.
        public string IPAddress { get; set; }
        public DateTime LastLogin { get; set; }

        public string? PushToken { get; set; }                     // FCM/APNs token
        public DateTime? PushTokenUpdatedAt { get; set; }
    }
}
