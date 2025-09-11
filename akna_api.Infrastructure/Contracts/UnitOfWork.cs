using akna_api.Infrastructure.Context;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Infrastructure.Contracts
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        // Lazy loading repositories
        private IUserRepository _users;
        private IRoleRepository _roles;
        private IPermissionRepository _permissions;
        private ISessionRepository _sessions;
        private IAuthenticationTokenRepository _authenticationTokens;
        private IVerificationRepository _verifications;
        private IDeviceInfoRepository _deviceInfos;
        private ITwoFactorAuthSettingRepository _twoFactorAuthSettings;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IRoleRepository Roles =>
            _roles ??= new RoleRepository(_context);

        public IPermissionRepository Permissions =>
            _permissions ??= new PermissionRepository(_context);

        public ISessionRepository Sessions =>
            _sessions ??= new SessionRepository(_context);

        public IAuthenticationTokenRepository AuthenticationTokens =>
            _authenticationTokens ??= new AuthenticationTokenRepository(_context);

        public IVerificationRepository Verifications =>
            _verifications ??= new VerificationRepository(_context);

        public IDeviceInfoRepository DeviceInfos =>
            _deviceInfos ??= new DeviceInfoRepository(_context);

        public ITwoFactorAuthSettingRepository TwoFactorAuthSettings =>
            _twoFactorAuthSettings ??= new TwoFactorAuthSettingRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
