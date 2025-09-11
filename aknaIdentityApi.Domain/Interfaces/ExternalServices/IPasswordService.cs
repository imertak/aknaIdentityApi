using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.ExternalServices
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        string GenerateRandomPassword(int length = 12);
    }
}
