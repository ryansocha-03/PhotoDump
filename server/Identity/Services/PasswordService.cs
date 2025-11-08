using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services;

public class PasswordService
{
    private static PasswordHasher<HashUser> _hasher = new();
    
    public bool PasswordHashMatches(string enteredPassword, string eventPasswordHash)
    {
        var hashUser = new HashUser();
        var hashMatchResult = _hasher.VerifyHashedPassword(hashUser, eventPasswordHash, enteredPassword);
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

    public string HashPassword(string enteredPassword)
    {
        var hashUser = new HashUser();
        return _hasher.HashPassword(hashUser, enteredPassword);
    }
}