using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Interfaces
{
    public interface IJwtProvider
    {
        public string GenerateToken(User user);
    }
}
