using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services;

public class PasswordService
{
    private static readonly PasswordHasher<object> Hasher = new();
    
    public bool PasswordHashMatches(string enteredPassword, string eventPasswordHash)
    {
        var hashMatchResult = Hasher.VerifyHashedPassword(null, eventPasswordHash, enteredPassword);
        switch (hashMatchResult)
        {
            case PasswordVerificationResult.Success:
                return true;
            case PasswordVerificationResult.Failed:
                return false;
            case PasswordVerificationResult.SuccessRehashNeeded:
                return true;
            default:
                return false;
        }
    }

    public string? VerifyPasswordRequirements(string enteredPassword)
    {
        return enteredPassword.Length < 6 ? "Password must be at least 6 characters" : null;
    }

    public string HashPassword(string enteredPassword)
    {
        return Hasher.HashPassword(null, enteredPassword);
    }
}