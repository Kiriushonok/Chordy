using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Login = user.Login,
                Id = user.Id,
            };
        }

        public static User ToEntity(UserRegisterDto userRegisterDto) 
        {
            return new User
            {
                Login = userRegisterDto.Login,
                PasswordHash = PasswordHasher.Generate(userRegisterDto.Password),
            };
        }
    }
}
