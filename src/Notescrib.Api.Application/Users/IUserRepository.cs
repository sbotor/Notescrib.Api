using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Users;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user, string password);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
}
