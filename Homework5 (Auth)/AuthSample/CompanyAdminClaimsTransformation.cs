using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AuthSample;

public class CompanyAdminClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity)clone.Identity!;

        // ВАШ КОД ТУТ:
        // 1. Отримайте email (ClaimTypes.Email).
        // 2. Якщо email закінчується на "@mycompany.com":
        // 3. Додайте новий claim (ClaimTypes.Role, "Admin").
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (email is not null && email.EndsWith("@mycompany.com"))
        {
            newIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        }

        return Task.FromResult(clone);
    }
}