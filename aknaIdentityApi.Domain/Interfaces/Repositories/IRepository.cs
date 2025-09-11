using akna_api.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetActiveAsync(); // IsDeleted = false olanlar
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id); // Hard delete
        Task SoftDeleteAsync(int id, int deletedByUserId); // Soft delete
        Task<bool> ExistsAsync(int id);
        Task<List<T>> GetByIdsAsync(List<int> ids);
        Task<int> CountAsync();
        Task<int> CountActiveAsync(); // IsDeleted = false olan sayısı
        Task SaveChangesAsync();
    }
}
