using TestsSample.Core.Infrastructure.Services;
using TestsSample.Core.Models;
using TestsSample.Core.Repositories;
using Dapper;

namespace TestsSample.Core.Infrastructure.Persistence.Repositories;

public class UserRepository(IDbConnectionFactory factory) : IRepository<User>
{
    private const string TableName = "Users";
    private const string Properties = "Id, Name, Age, Email, HashedPassword";
    private static readonly string ValueParams = string.Join(", ", 
                                                        Properties.Split(", ")
                                                            .Select(p => $"@{p}"));
    private static readonly string UpdateProperties = string.Join(", ", 
                                                        Properties.Split(", ")
                                                            .Where(p => p != "Id")
                                                            .Select(p => $"{p} = @{p}"));
    private const string SelectAll = $"SELECT * FROM {TableName}";
    private const string SelectUser = $"SELECT * FROM {TableName} WHERE Id = @Id";
    private static readonly string InsertUser = $"INSERT INTO {TableName} ({Properties}) VALUES ({ValueParams})";
    private static readonly string UpdateUser = $"UPDATE {TableName} SET {UpdateProperties} WHERE Id = @Id";
    private const string DeleteUser = $"DELETE FROM {TableName} WHERE Id = @Id";
    
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        var connection = factory.CreateConnection();
        var command = new CommandDefinition(
            commandText: SelectAll, 
            cancellationToken: ct);
        
        return await connection.QueryAsync<User>(command);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var connection = factory.CreateConnection();
        var command = new CommandDefinition(
            commandText: SelectUser, 
            parameters: new { Id = id },
            cancellationToken: ct);
        
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    public async Task<bool> AddAsync(User entity, CancellationToken ct = default)
    {
        var connection = factory.CreateConnection();
        var command = new CommandDefinition(
            commandText: InsertUser,
            parameters: entity,
            cancellationToken: ct);
        
        var rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(User entity, CancellationToken ct = default)
    {
        var connection = factory.CreateConnection();
        var command = new CommandDefinition(
            commandText: UpdateUser,
            parameters: entity,
            cancellationToken: ct);
        
        var rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(User entity, CancellationToken ct = default)
    {
        var connection = factory.CreateConnection();
        var command = new CommandDefinition(
            commandText: DeleteUser,
            parameters: new { Id = entity.Id },
            cancellationToken: ct);
        
        var rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected > 0;
    }
}