using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Negade.Domain.Entities;

namespace Negade.Infrastructure.Auth;

public class LegacyCompatiblePasswordHasher : PasswordHasher<AppUser>
{
    public override PasswordVerificationResult VerifyHashedPassword(
        AppUser user,
        string hashedPassword,
        string providedPassword)
    {
        var identityResult = base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        if (identityResult != PasswordVerificationResult.Failed)
        {
            return identityResult;
        }

        return VerifyLegacyPassword(providedPassword, hashedPassword)
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Failed;
    }

    private static bool VerifyLegacyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[1]);
            var expectedKey = Convert.FromBase64String(parts[2]);
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
