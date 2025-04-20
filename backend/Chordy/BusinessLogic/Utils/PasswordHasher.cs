using BCrypt.Net;

namespace Chordy.BusinessLogic.Utils
{
    public static class PasswordHasher
    {
        public static string Generate(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public static bool Verify(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
