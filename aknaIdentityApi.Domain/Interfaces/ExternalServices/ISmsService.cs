using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.ExternalServices
{
    public interface ISmsService
    {
        Task SendSmsAsync(string to, string message);
        Task SendVerificationSmsAsync(string to, string verificationCode);
    }
}
