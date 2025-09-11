using aknaIdentity_api.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
        ISessionRepository Sessions { get; }
        IAuthenticationTokenRepository AuthenticationTokens { get; }
        IVerificationRepository Verifications { get; }
        IDeviceInfoRepository DeviceInfos { get; }
        ITwoFactorAuthSettingRepository TwoFactorAuthSettings { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
