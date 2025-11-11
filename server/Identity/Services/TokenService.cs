using Identity.Models;

namespace Identity.Services;

public class TokenService
{
    public string CreateAnonymousGuestToken(Guid eventPublicId)
    {
        return $"{eventPublicId.ToString()}-{Roles.Anonymous}";
    }
}