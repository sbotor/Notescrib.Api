﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Api.Application.Common.Configuration;

namespace Notescrib.Api.Application.Auth.Services;

internal class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _settings;

    public JwtProvider(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrEmpty(_settings.Key))
        {
            throw new InvalidOperationException("No JWT key provided.");
        }
    }

    public string GenerateToken(string userId, string userEmail)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, userEmail)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var descriptor = new JwtSecurityToken(
            issuer: _settings.Issuer,
            claims: claims,
            expires: DateTime.Now + _settings.TokenLifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }
}