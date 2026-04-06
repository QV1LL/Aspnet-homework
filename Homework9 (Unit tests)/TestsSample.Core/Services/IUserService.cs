using FluentResults;

namespace TestsSample.Core.Services;

public interface IUserService
{
    Task<Result> CreateAsync(string name, int age, string email, string password, CancellationToken ct = default);
    Task<Result> ChangePasswordAsync(Guid id, string oldPassword, string newPassword, CancellationToken ct = default);
    Task<Result> ChangeEmailAsync(Guid id, string password, string newEmail, CancellationToken ct = default);
    Task<Result> UpdateMetadataAsync(Guid id, string name, int age, CancellationToken ct = default);
    Task<Result> RemoveAsync(Guid id, string password, CancellationToken ct = default);
}