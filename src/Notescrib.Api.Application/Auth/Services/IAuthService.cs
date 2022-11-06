using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Auth.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
}
