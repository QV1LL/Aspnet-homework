using Dapper;
using TestsSample.Core.Infrastructure.Persistence.Repositories;
using TestsSample.Core.Models;

namespace TestsSample.Tests.Infrastructure.Persistence.Repositories;

public class UserRepositoryTests : IClassFixture<DbConnectionFactoryFixture>
{
    private readonly UserRepository _repository;
    private readonly DbConnectionFactoryFixture _fixture;

    public UserRepositoryTests(DbConnectionFactoryFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(fixture.FactoryMock.Object);
        _fixture.Connection.Execute("DELETE FROM Users;");
    }

    [Fact]
    public async Task GetAllUsers_ContainsUsers_ReturnsUsers()
    {
        var insertUsersSql = """

                                 INSERT INTO Users (Id, Name, Email, Age, HashedPassword)
                                 VALUES 
                                     ('550e8400-e29b-41d4-a716-446655440000', 'John Doe', 'john@example.com', 28, 'a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6'),
                                     ('6ba7b810-9dad-11d1-80b4-00c04fd430c8', 'Jane Smith', 'jane@test.com', 34, 'z9y8x7w6v5u4t3s2r1q0p1o2n3m4l5k6'),
                                     ('ad6bd510-c035-4299-8e50-256196236b2d', 'Omar', 'omar@gmail.com', 20, 'q1w2e3r4t5y6u7i8o9p0a1s2d3f4g5h6');

                             """;
        await _fixture.Connection.ExecuteAsync(insertUsersSql);

        var users = await _repository.GetAllAsync();

        users.Should()
            .NotBeEmpty().And.HaveCount(3);
    }

    [Fact]
    public async Task GetByIdAsync_ContainUser_ReturnsUser()
    {
        var id = await InsertUserAsync();
        
        var user = await _repository.GetByIdAsync(id);

        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DoesNotContainUser_ReturnsNull()
    {
        var id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        
        var user = await _repository.GetByIdAsync(id);

        user.Should().BeNull();
    }
    
    [Fact]
    public async Task AddAsync_ValidUser_ReturnsTrue()
    {
        var user = User.Create("John Doe", "john@example.com", 28, "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6").Value;

        var result = await _repository.AddAsync(user);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_ValidUser_UserIsPersisted()
    {
        var user = User.Create("John Doe", "john@example.com", 28, "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6").Value;

        await _repository.AddAsync(user);

        var persisted = await _repository.GetByIdAsync(user.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(user.Name);
        persisted.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ReturnsTrue()
    {
        var id = await InsertUserAsync();
        
        var user = await _repository.GetByIdAsync(id);
        user!.UpdateMetadata("John Updated", 30);

        var result = await _repository.UpdateAsync(user);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ChangesArePersisted()
    {
        var id = await InsertUserAsync();
        
        var user = await _repository.GetByIdAsync(id);
        user!.UpdateMetadata("John Updated", 30);
        await _repository.UpdateAsync(user);

        var updated = await _repository.GetByIdAsync(id);
        updated!.Name.Should().Be("John Updated");
        updated.Age.Should().Be(30);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ReturnsFalse()
    {
        var user = User.Create("Ghost", "ghost@example.com", 99, "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6").Value;

        var result = await _repository.UpdateAsync(user);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_ReturnsTrue()
    {
        var id = await InsertUserAsync();
        
        var user = await _repository.GetByIdAsync(id);
        var result = await _repository.DeleteAsync(user!);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_UserIsRemoved()
    {
        var id = await InsertUserAsync();
        
        var user = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(user!);

        var deleted = await _repository.GetByIdAsync(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        var user = User.Create("Ghost", "ghost@example.com", 99, "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6").Value;

        var result = await _repository.DeleteAsync(user);

        result.Should().BeFalse();
    }
    
    private async Task<Guid> InsertUserAsync(string name = "John Doe", string email = "john@example.com", int age = 28)
    {
        var id = Guid.NewGuid();
        await _fixture.Connection.ExecuteAsync(
            "INSERT INTO Users (Id, Name, Email, Age, HashedPassword) VALUES (@Id, @Name, @Email, @Age, @HashedPassword)",
            new { Id = id, Name = name, Email = email, Age = age, HashedPassword = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6" });
        return id;
    }
}