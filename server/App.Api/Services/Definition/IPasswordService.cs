namespace App.Api.Services.Definition;

/// <summary>
/// Service interface defining operations for hashing and validating passwords.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Determines whether a provided plain-text password matches a given hash.
    /// </summary>
    /// <param name="password">The plain-text password to verify against the hash.</param>
    /// <param name="hash">The password hash to compare against.</param>
    /// <returns></returns>
    bool PasswordMatchesHash(string password, string hash);
    
    /// <summary>
    /// Hashes a provided password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Determines whether the provided plain-text password meets basic security requirements.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <exception cref="ArgumentException">If the password does not meet basic security requirements.</exception>
    void PasswordMeetsRequirements(string password);
}