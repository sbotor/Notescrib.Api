using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Auth.Services;

public interface IAuthService
{
    Task<Result<UserDetails>> AuthenticateAsync(string email, string password);
}
