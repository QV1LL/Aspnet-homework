using System.Text.RegularExpressions;
using FluentResults;

namespace TestsSample.Core.Models;

public class User
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private const int MinHashLength = 32;

    public Guid Id { get; private set; }

    public string Name
    {
        get;
        private set => field = string.IsNullOrWhiteSpace(value) 
            ? throw new ArgumentException("Value cannot be null or whitespace.", nameof(value)) 
            : value;
    }

    public int Age
    {
        get;
        private set => field = value is <= 0 or > 120
            ? throw new ArgumentOutOfRangeException(nameof(value), value, null) 
            : value;
    }

    public string Email 
    { 
        get; 
        private set => field = !EmailRegex.IsMatch(value) 
            ? throw new ArgumentException("Invalid email format.", nameof(value)) 
            : value; 
    }

    public string HashedPassword 
    { 
        get; 
        private set => field = string.IsNullOrEmpty(value) || value.Length < MinHashLength 
            ? throw new ArgumentException("Password hash is too short or empty.", nameof(value)) 
            : value; 
    }

    public User() { }

    public static Result<User> Create(string name, string email, int age, string hashedPassword)
    {
        try
        {
            return Result.Ok(new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Age = age,
                HashedPassword = hashedPassword
            });
        }
        catch (Exception ex)
        {
            return Result.Fail<User>(ex.Message);
        }
    }

    public Result UpdateMetadata(string newName, int newAge)
    {
        var currentMetadata = new { Name = Name, Age = Age };
        
        try
        {
            Name = newName;
            Age = newAge;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Name = currentMetadata.Name;
            Age = currentMetadata.Age;
            return Result.Fail(ex.Message);
        }
    }

    public Result ChangeEmail(string newEmail)
    {
        try
        {
            Email = newEmail;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public Result ChangePassword(string newHashedPassword)
    {
        try
        {
            HashedPassword = newHashedPassword;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}