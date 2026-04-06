using TestsSample.Core.Models;

namespace TestsSample.Tests.Models;

public class UserTests
{
    [Fact]
    public void Create_ValidUserData_ReturnsSuccessResult()
    {
        var userData = new
        {
            Name = "Omar",
            Age = 20,
            Email = "omar@gmail.com",
            HashedPassword = "SomeLongPasswordHashThatMustLookLikeRealAndDontFailTest",
        };

        var result = User.Create(userData.Name, userData.Email, userData.Age, userData.HashedPassword);
        
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(UserTestData))]
    public void Create_InvalidUserData_ReturnsFailureResult(
        string name, string email, int age, string hashedPassword)
    {
        var result = User.Create(name, email, age, hashedPassword);
        
        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public void UpdateMetadata_ValidData_ReturnsSuccessAndUpdatesState()
    {
        var user = User.Create("John", "john@gmail.com", 25, new string('a', 32)).Value;

        var result = user.UpdateMetadata("Jane", 30);

        result.IsSuccess.Should().BeTrue();
        user.Name.Should().Be("Jane");
        user.Age.Should().Be(30);
    }

    [Theory]
    [InlineData("", 25)]
    [InlineData("Jane", 0)]
    [InlineData("Jane", 121)]
    [InlineData(null, -5)]
    public void UpdateMetadata_InvalidData_ReturnsFailure(string newName, int newAge)
    {
        var user = User.Create("John", "john@gmail.com", 25, new string('a', 32)).Value;

        var result = user.UpdateMetadata(newName, newAge);

        result.IsFailed.Should().BeTrue();
        user.Name.Should().Be("John");
        user.Age.Should().Be(25);
    }

    [Fact]
    public void ChangeEmail_ValidEmail_ReturnsSuccessAndUpdatesEmail()
    {
        var user = User.Create("John", "john@gmail.com", 25, new string('a', 32)).Value;

        var result = user.ChangeEmail("new@gmail.com");

        result.IsSuccess.Should().BeTrue();
        user.Email.Should().Be("new@gmail.com");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("")]
    [InlineData("test@")]
    public void ChangeEmail_InvalidEmail_ReturnsFailure(string newEmail)
    {
        var user = User.Create("John", "john@gmail.com", 25, new string('a', 32)).Value;

        var result = user.ChangeEmail(newEmail);

        result.IsFailed.Should().BeTrue();
        user.Email.Should().Be("john@gmail.com");
    }

    [Fact]
    public void ChangePassword_ValidHash_ReturnsSuccessAndUpdatesPassword()
    {
        var user = User.Create("John", "john@gmail.com", 25, new string('a', 32)).Value;
        var newHash = new string('b', 32);

        var result = user.ChangePassword(newHash);

        result.IsSuccess.Should().BeTrue();
        user.HashedPassword.Should().Be(newHash);
    }

    [Theory]
    [InlineData("too-short")]
    [InlineData("")]
    [InlineData(null)]
    public void ChangePassword_InvalidHash_ReturnsFailure(string newHash)
    {
        var hashedPassword = new string('a', 32);
        var user = User.Create("John", "john@gmail.com", 25, hashedPassword).Value;

        var result = user.ChangePassword(newHash);

        result.IsFailed.Should().BeTrue();
        user.HashedPassword.Should().Be(hashedPassword);
    }
}