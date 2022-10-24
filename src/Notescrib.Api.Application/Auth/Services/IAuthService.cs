using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Auth.Services;

public interface IAuthService
{
    Task<Result<User>> AuthenticateAsync(string email, string password);
}
