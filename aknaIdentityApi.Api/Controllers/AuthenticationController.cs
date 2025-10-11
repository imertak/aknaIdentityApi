using aknaIdentityApi.Domain.Dtos.Requests;
using aknaIdentityApi.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentityApi.Api.Controllers
{
    [ApiController]
    [Route("api/authentications")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("add-user")]
        public async Task AddUserAsync([FromBody] UserRegisterRequest request) 
        {
            await authenticationService.RegisterAsync(request);
        }
    }
}
