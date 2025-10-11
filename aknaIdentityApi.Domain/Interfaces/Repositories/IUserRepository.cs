
using aknaIdentityApi.Domain.Dtos.Requests;

namespace aknaIdentityApi.Domain.Interfaces.Repositories
{
    /// <summary>
    /// IUserRepository
    /// </summary>
    public interface IUserRepository
    {

        /// <summary>
        /// Kullanıcı kaydını sağlar
        /// </summary>
        /// <param name="request">UserRegisterRequest</param>
        /// <returns></returns>
        Task<long> AddUserAsync(UserRegisterRequest request);
    }
}
