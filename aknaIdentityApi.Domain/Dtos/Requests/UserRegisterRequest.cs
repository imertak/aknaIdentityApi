
using aknaIdentityApi.Domain.Enums;

namespace aknaIdentityApi.Domain.Dtos.Requests
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int TurkishRepublicIdNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public GenderType Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public UserType UserType { get; set; }
        public string DeviceId { get; set; } = default!;           // UUID/IMEI/Firebase Instance ID
        public string DeviceType { get; set; }                     // iOS, Android, Web
        public string? DeviceModel { get; set; }                  // iPhone 12, Samsung Galaxy S21, etc.
        public string IPAddress { get; set; }
        public string? PushToken { get; set; }                     // FCM/APNs token

    }
}
