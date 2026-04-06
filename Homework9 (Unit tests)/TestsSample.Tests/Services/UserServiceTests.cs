using TestsSample.Core.Infrastructure.Services;
using TestsSample.Core.Models;
using TestsSample.Core.Repositories;
using TestsSample.Core.Services;

namespace TestsSample.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IRepository<User>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _service = new UserService(_repositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsSuccessResult()
    {
        var password = "SafePassword123";
        var validHash = new string('h', 32);
        _passwordHasherMock.Setup(x => x.Hash(password)).Returns(validHash);
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        var result = await _service.CreateAsync("John Doe", 25, "john@test.com", password);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", 25, "test@test.com", "password")]
    [InlineData("John", -1, "test@test.com", "password")]
    [InlineData("John", 25, "invalid-email", "password")]
    [InlineData("John", 25, "test@test.com", "")]
    public async Task CreateAsync_InvalidData_ReturnsFailResult(string name, int age, string email, string password)
    {
        var result = await _service.CreateAsync(name, age, email, password);

        result.IsFailed.Should().BeTrue();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SuccessCreate_IsPasswordHashed()
    {
        var rawPassword = "raw_password";
        var expectedHash = new string('x', 32);
        User capturedUser = null!;

        _passwordHasherMock.Setup(x => x.Hash(rawPassword)).Returns(expectedHash);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, ct) => capturedUser = u)
            .ReturnsAsync(true);

        await _service.CreateAsync("John", 30, "john@test.com", rawPassword);

        capturedUser.Should().NotBeNull();
        capturedUser.HashedPassword.Should().Be(expectedHash);
    }

    [Fact]
    public async Task CreateAsync_RepositoryReturnsFalse_ReturnsFail()
    {
        var validHash = new string('h', 32);
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns(validHash);
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

        var result = await _service.CreateAsync("John", 20, "test@test.com", "password");

        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ValidData_ReturnsSuccessResult()
    {
        var id = Guid.NewGuid();
        var oldPassword = "OldPassword123";
        var newPassword = "NewPassword123";
        var oldHash = new string('o', 32);
        var newHash = new string('n', 32);
        var user = User.Create("John", "john@test.com", 25, oldHash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(oldHash, oldPassword))
                           .Returns(true);
        _passwordHasherMock.Setup(x => x.Hash(newPassword))
                           .Returns(newHash);
        _repositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        var result = await _service.ChangePasswordAsync(id, oldPassword, newPassword);

        result.IsSuccess.Should().BeTrue();
        user.HashedPassword.Should().Be(newHash);
    }

    [Fact]
    public async Task ChangePasswordAsync_UserNotFound_ReturnsFailResult()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((User?)null);

        var result = await _service.ChangePasswordAsync(id, "any", "any");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_WrongOldPassword_ReturnsFailResult()
    {
        var id = Guid.NewGuid();
        var oldHash = new string('o', 32);
        var user = User.Create("John", "john@test.com", 25, oldHash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(oldHash, It.IsAny<string>()))
                           .Returns(false);

        var result = await _service.ChangePasswordAsync(id, "wrong_password", "new_password");

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidNewPassword_ReturnsFailResult()
    {
        var id = Guid.NewGuid();
        var oldHash = new string('o', 32);
        var user = User.Create("John", "john@test.com", 25, oldHash).Value;
        var invalidShortHash = "too_short";

        _repositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(oldHash, It.IsAny<string>()))
                           .Returns(true);
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>()))
                           .Returns(invalidShortHash);

        var result = await _service.ChangePasswordAsync(id, "old", "new");

        result.IsFailed.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_UpdateFails_ReturnsFailResult()
    {
        var id = Guid.NewGuid();
        var oldHash = new string('o', 32);
        var user = User.Create("John", "john@test.com", 25, oldHash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(true);
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>()))
                           .Returns(new string('n', 32));
        _repositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

        var result = await _service.ChangePasswordAsync(id, "old", "new");

        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task ChangeEmailAsync_ValidData_ReturnsSuccess()
    {
        var hash = new string('a', 32);
        var user = User.Create("John", "old@test.com", 25, hash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(hash, "pass"))
            .Returns(true);
        _repositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ChangeEmailAsync(Guid.NewGuid(), "pass", "new@test.com");

        result.IsSuccess.Should().BeTrue();
        user.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task ChangeEmailAsync_WrongPassword_ReturnsFail()
    {
        var hash = new string('a', 32);
        var user = User.Create("John", "old@test.com", 25, hash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(hash, "wrong"))
            .Returns(false);

        var result = await _service.ChangeEmailAsync(Guid.NewGuid(), "wrong", "new@test.com");

        result.IsFailed.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangeEmailAsync_InvalidEmailFormat_ReturnsFail()
    {
        var hash = new string('a', 32);
        var user = User.Create("John", "old@test.com", 25, hash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var result = await _service.ChangeEmailAsync(Guid.NewGuid(), "pass", "invalid");

        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateMetadataAsync_ValidData_ReturnsSuccess()
    {
        var user = User.Create("Old", "t@t.com", 20, new string('a', 32)).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.UpdateMetadataAsync(Guid.NewGuid(), "New", 30);

        result.IsSuccess.Should().BeTrue();
        user.Name.Should().Be("New");
        user.Age.Should().Be(30);
    }

    [Theory]
    [InlineData("", 25)]
    [InlineData("John", -1)]
    public async Task UpdateMetadataAsync_InvalidData_ReturnsFail(string name, int age)
    {
        var user = User.Create("John", "t@t.com", 25, new string('a', 32)).Value;
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.UpdateMetadataAsync(Guid.NewGuid(), name, age);

        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    public async Task RemoveAsync_ValidPassword_ReturnsSuccess()
    {
        var hash = new string('a', 32);
        var user = User.Create("John", "t@t.com", 25, hash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(hash, "pass"))
            .Returns(true);
        _repositoryMock.Setup(x => x.DeleteAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.RemoveAsync(Guid.NewGuid(), "pass");

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WrongPassword_ReturnsFail()
    {
        var hash = new string('a', 32);
        var user = User.Create("John", "t@t.com", 25, hash).Value;

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(hash, "wrong"))
            .Returns(false);

        var result = await _service.RemoveAsync(Guid.NewGuid(), "wrong");

        result.IsFailed.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}