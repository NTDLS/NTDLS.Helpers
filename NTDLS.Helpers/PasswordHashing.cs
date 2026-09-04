using System.Security.Cryptography;

namespace NTDLS.Helpers
{
    /// <summary>
    /// PBKDF2-SHA256 password hashing (salt + iteration count embedded in the stored hash, so the
    /// work factor can be raised later without a migration). No external Identity package needed.
    /// </summary>
    public static class PasswordHashing
    {
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const int Iterations = 210000;

        /// <summary>
        /// Hashes a password using PBKDF2-SHA256.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSizeBytes);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }
        
        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="passwordHash">The hashed password.</param>
        /// <returns>True if the password matches the hashed password; otherwise, false.</returns>
        public static bool Verify(string password, string passwordHash)
        {
            var parts = passwordHash.Split('.', 3);
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[1]);
            var expectedHash = Convert.FromBase64String(parts[2]);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
