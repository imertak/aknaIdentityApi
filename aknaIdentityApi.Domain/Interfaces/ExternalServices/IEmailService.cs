using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.ExternalServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendVerificationEmailAsync(string to, string verificationCode);
        Task SendPasswordResetEmailAsync(string to, string resetToken);
    }
}
