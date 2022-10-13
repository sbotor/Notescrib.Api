using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Services;

public interface IUserService
{
    Task<Result<UserDetails>> AddUserAsync(CreateUserRequest request);
    Task<Result<bool>> CheckEmailAsync(string email);
    Task<Result<UserDetails>> GetUserByEmailAsync(string email);
}
