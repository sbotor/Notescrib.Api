using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core;

namespace Notescrib.Api.Application.Services;

public interface IUserService
{
    Task<ApiResponse<UserDetails>> AddUserAsync(CreateUserRequest request);
    Task<ApiResponse<bool>> CheckEmailAsync(string email);
    Task<ApiResponse<UserDetails>> GetUserByEmailAsync(string email);
    Task<ApiResponse<UserDetails>> VerifyCredentialsAsync(LoginRequest request);
}
