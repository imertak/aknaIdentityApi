using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Dtos
{
    // Authentication
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceInfo { get; set; }
        public bool RememberMe { get; set; }
    }
}
