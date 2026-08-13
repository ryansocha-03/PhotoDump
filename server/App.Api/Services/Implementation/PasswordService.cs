using App.Api.Services.Definition;
using Microsoft.AspNetCore.Identity;

namespace App.Api.Services.Implementation;

/// <summary>
/// Service implementation for functions relating to hashing and validating passwords.
/// </summary>
public class PasswordService(IPasswordHasher<object> passwordHasher) : IPasswordService
{
    /// <inheritdoc /> 
    public bool PasswordMatchesHash(string password, string hash)
    {
        var hashResult = passwordHasher.VerifyHashedPassword(new {},  hash, hash);
        return hashResult switch
        {
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            _ => false
        };
    }

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        return passwordHasher.HashPassword(new { }, password);
    }

    /// <inheritdoc />
    public void PasswordMeetsRequirements(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password must not be empty");
        }
        else if (password.Length < 6)
        {
            throw new ArgumentException("Password must have at least 6 characters");
        }
    }
}