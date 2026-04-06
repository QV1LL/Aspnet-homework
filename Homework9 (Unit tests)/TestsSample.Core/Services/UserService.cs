using FluentResults;
using TestsSample.Core.Infrastructure.Services;
using TestsSample.Core.Models;
using TestsSample.Core.Repositories;

namespace TestsSample.Core.Services;

public class UserService(
    IRepository<User> repository, 
    IPasswordHasher passwordHasher) : IUserService
{
    private const string PasswordEmptyMessage = "Password is empty.";
    private const string NotFoundMessage = "User not found.";
    private const string AuthenticationFailedMessage = "Authentication failed.";
    private const string UpdateFailedMessage = "User update failed.";
    private const string CreationFailedMessage = "User creation failed.";
    private const string DeleteFailedMessage = "Failed to delete account.";

    public async Task<Result> CreateAsync(string name, int age, string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(password)) return Result.Fail(PasswordEmptyMessage);
        
        var hashedPassword = passwordHasher.Hash(password);
        var userResult = User.Create(name, email, age, hashedPassword);

        if (userResult.IsFailed) return userResult.ToResult();

        return await repository.AddAsync(userResult.Value, ct) 
            ? Result.Ok() 
            : Result.Fail(CreationFailedMessage);
    }

    public async Task<Result> ChangePasswordAsync(Guid id, string oldPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        if (user == null) return Result.Fail(NotFoundMessage);

        if (!passwordHasher.Verify(user.HashedPassword, oldPassword))
            return Result.Fail(AuthenticationFailedMessage);

        var updateResult = user.ChangePassword(passwordHasher.Hash(newPassword));
        if (updateResult.IsFailed) return updateResult;

        return await repository.UpdateAsync(user, ct) 
            ? Result.Ok() 
            : Result.Fail(UpdateFailedMessage);
    }

    public async Task<Result> ChangeEmailAsync(Guid id, string password, string newEmail, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        if (user == null) return Result.Fail(NotFoundMessage);

        if (!passwordHasher.Verify(user.HashedPassword, password))
            return Result.Fail(AuthenticationFailedMessage);

        var updateResult = user.ChangeEmail(newEmail);
        if (updateResult.IsFailed) return updateResult;

        return await repository.UpdateAsync(user, ct) 
            ? Result.Ok() 
            : Result.Fail(UpdateFailedMessage);
    }

    public async Task<Result> UpdateMetadataAsync(Guid id, string name, int age, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        if (user == null) return Result.Fail(NotFoundMessage);

        var updateResult = user.UpdateMetadata(name, age);
        if (updateResult.IsFailed) return updateResult;

        return await repository.UpdateAsync(user, ct) 
            ? Result.Ok() 
            : Result.Fail(UpdateFailedMessage);
    }

    public async Task<Result> RemoveAsync(Guid id, string password, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        if (user == null) return Result.Fail(NotFoundMessage);

        if (!passwordHasher.Verify(user.HashedPassword, password))
            return Result.Fail($"{AuthenticationFailedMessage} {DeleteFailedMessage}");

        return await repository.DeleteAsync(user, ct) 
            ? Result.Ok() 
            : Result.Fail(DeleteFailedMessage);
    }
}