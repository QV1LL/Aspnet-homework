using _2FaSample.Features.Profile.GetUserProfile;

namespace _2FaSample.Features.Profile;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoint(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/profile")
            .RequireAuthorization();

        group.MapGet("/", GetUserProfileHandler.Handle);
    }
}