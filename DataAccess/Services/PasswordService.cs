using System.Security.Cryptography;

namespace Sln.DataAccess.Services;

public class PasswordService
{
    public (string hash, string salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(32);
        var salt = Convert.ToBase64String(saltBytes);

        var hashBytes = new Rfc2898DeriveBytes(
            password,
            saltBytes,
            100000,
            HashAlgorithmName.SHA256
        ).GetBytes(32);

        var hash = Convert.ToBase64String(hashBytes);

        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var hashBytes = new Rfc2898DeriveBytes(
            password,
            saltBytes,
            100000,
            HashAlgorithmName.SHA256
        ).GetBytes(32);

        var computedHash = Convert.ToBase64String(hashBytes);

        return computedHash == hash;
    }
}