using System.Security.Cryptography;
using Litrater.Application.Abstractions.Authentication;

namespace Litrater.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return Convert.ToBase64String(salt.Concat(hash).ToArray());
    }

    public bool Verify(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);
        var salt = hashBytes.Take(SaltSize).ToArray();
        var storedHash = hashBytes.Skip(SaltSize).ToArray();

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return computedHash.SequenceEqual(storedHash);
    }
}
