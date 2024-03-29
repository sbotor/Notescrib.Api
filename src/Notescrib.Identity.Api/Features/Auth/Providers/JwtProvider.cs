﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Identity.Features.Auth.Providers;

namespace Notescrib.Identity.Api.Features.Auth.Providers;

internal class JwtProvider : IJwtProvider
{
    private readonly IdentityJwtSettings _settings;

    public JwtProvider(IOptions<IdentityJwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken(string userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var descriptor = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.Now + _settings.TokenLifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }
}
