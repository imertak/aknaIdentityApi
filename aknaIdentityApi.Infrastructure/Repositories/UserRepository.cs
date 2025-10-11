using aknaIdentityApi.Domain.Dtos.Requests;
using aknaIdentityApi.Domain.Entities;
using aknaIdentityApi.Domain.Interfaces.Repositories;
using aknaIdentityApi.Infrastructure.Contexts;


namespace aknaIdentityApi.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AknaIdentityDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kullanıcı kaydını sağlar
        /// </summary>
        /// <param name="request">UserRegisterRequest</param>
        /// <returns></returns>
        public async Task<long> AddUserAsync(UserRegisterRequest request)
        {
            User user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PasswordHash = request.Password,
                PhoneNumber = request.PhoneNumber,
                BloodType = request.BloodType,
                TurkishRepublicIdNumber = request.TurkishRepublicIdNumber,
                Address = request.Address,
                BirthDate = request.BirthDate,
                UserType = request.UserType,
                CreatedUser = "system",
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user.Id;
        }
    }
}
