using TestsSample.Core.Infrastructure.Services;

namespace TestsSample.Tests.Infrastructure.Services;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher;
    
    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }
    
    [Fact]
    public void Hash_ValidPassword_ReturnsHashedString()
    {
        var password = "SafePassword123";

        var result = _passwordHasher.Hash(password);

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().NotBe(password);
    }

    [Fact]
    public void Verify_MatchingPasswordAndHash_ReturnsTrue()
    {
        var password = "CorrectPassword";
        var hash = _passwordHasher.Hash(password);

        var result = _passwordHasher.Verify(hash, password);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_NonMatchingPasswordAndHash_ReturnsFalse()
    {
        var password = "CorrectPassword";
        var hash = _passwordHasher.Hash(password);

        var result = _passwordHasher.Verify(hash, "WrongPassword");

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Hash_InvalidInput_ThrowsException(string invalidPassword)
    {
        var act = () => _passwordHasher.Hash(invalidPassword);

        act.Should().Throw<Exception>();
    }
}