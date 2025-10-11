using aknaIdentityApi.Domain.Interfaces.UnitOfWorks;
using aknaIdentityApi.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore.Storage;


namespace aknaIdentityApi.Infrastructure.UnitOfWorks
{
    /// <summary>
    /// Unit of Work implementation
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AknaIdentityDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AknaIdentityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Synchronously saves all changes made in this context to the database
        /// </summary>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the context
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
