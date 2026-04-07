using System.Security.Claims;

namespace NotesApp.Api.Features.Helpers;

public static class UserHelper
{
    extension(ClaimsPrincipal user)
    {
        public Guid? GetId()
        {
            var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(idClaim, out var id))
            {
                return id;
            }

            return null;
        }

        public Guid GetRequiredId()
        {
            var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? throw new UnauthorizedAccessException("User identifier claim is missing.");

            return Guid.Parse(idClaim);
        }
    }
}