
using aknaIdentityApi.Domain.Dtos.Requests;
using aknaIdentityApi.Domain.Interfaces.Repositories;
using aknaIdentityApi.Domain.Interfaces.Services;

namespace aknaIdentityApi.Business.Services
{
    /// <summary>
    /// AuthenticationService
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        public AuthenticationService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// RegisterAsync
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task RegisterAsync(UserRegisterRequest request)
        {
            await userRepository.AddUserAsync(request);
        }
    }
}
