using _2FaSample.Enums;

namespace _2FaSample.Features.Auth.TwoFactor.Verify;

public record VerifyTwoFactorRequest(string UserId, string Code, string Method);