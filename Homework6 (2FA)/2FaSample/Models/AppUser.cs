using _2FaSample.Enums;
using Microsoft.AspNetCore.Identity;

namespace _2FaSample.Models;

public class AppUser : IdentityUser
{
    public List<TwoFactorMethod> EnabledTwoFactorMethods { get; set; } = [];
}