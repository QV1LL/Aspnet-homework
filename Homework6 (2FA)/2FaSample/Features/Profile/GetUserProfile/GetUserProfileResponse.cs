using _2FaSample.Enums;

namespace _2FaSample.Features.Profile.GetUserProfile;

public record GetUserProfileResponse(
    string UserName,
    string? PhoneNumber,
    List<TwoFactorMethod> EnabledTwoFactorMethods
);