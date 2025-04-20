using System.Security.Cryptography;

namespace Chordy.BusinessLogic.Utils
{
    public static class TokenHelper
    {
        public static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
