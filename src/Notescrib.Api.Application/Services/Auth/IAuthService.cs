using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core;

namespace Notescrib.Api.Application.Services.Auth;
public interface IAuthService
{
    Task<ApiResponse<TokenResponse>> AuthenticateAsync(LoginRequest request);
}
