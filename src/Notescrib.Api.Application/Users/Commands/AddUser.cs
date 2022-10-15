using AutoMapper;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Users.Commands;

public static class AddUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : ICommand<Result<UserDetails>>;

    internal class Handler : ICommandHandler<Command, Result<UserDetails>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public Handler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserDetails>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                return Result<UserDetails>.Failure("Passwords do not match.");
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return Result<UserDetails>.Failure("User with this email already exists.");
            }

            var user = new User
            {
                Email = request.Email,
                IsActive = true,
            };

            user = await _userRepository.AddUserAsync(user, request.Password);

            return Result<UserDetails>.Created(_mapper.Map<UserDetails>(user));
        }
    }
}
