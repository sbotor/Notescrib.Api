using Notescrib.Api.Application.Auth.Models;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Users.Mappers;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Users.Commands;

public static class CreateUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : ICommand<TokenResponse>;

    internal class Handler : ICommandHandler<Command, TokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMapper _mapper;
        private readonly IJwtProvider _jwtProvider;

        public Handler(IUserRepository userRepository, IUserMapper mapper, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
        }

        public async Task<TokenResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                throw new AppException("Passwords do not match.");
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new NotFoundException("User with this email already exists.");
            }

            var user = _mapper.MapToEntity(request);

            user = await _userRepository.AddUserAsync(user, request.Password);

            return new TokenResponse
            {
                Token = _jwtProvider.GenerateToken(user.Id, user.Email),
                User = _mapper.MapToDetails(user)
            };
        }
    }
}
