
using aknaIdentityApi.Domain.Dtos.Requests;

namespace aknaIdentityApi.Domain.Interfaces.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// RegisterAsync
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task RegisterAsync(UserRegisterRequest request);
    }
}
