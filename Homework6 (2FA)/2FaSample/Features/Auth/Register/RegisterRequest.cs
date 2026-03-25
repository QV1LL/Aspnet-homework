using _2FaSample.Enums;

namespace _2FaSample.Features.Auth.Register;

public record RegisterRequest(string UserName, string Password, string PhoneNumber);