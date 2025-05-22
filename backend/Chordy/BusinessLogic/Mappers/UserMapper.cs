using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Utils;
using Chordy.DataAccess.Entities;
using System.Linq;

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
                Role = user.UserRoles.FirstOrDefault()?.Role.Name
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
