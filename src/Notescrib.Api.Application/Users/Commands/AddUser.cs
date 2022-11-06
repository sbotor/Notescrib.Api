using AutoMapper;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Users.Commands;

public static class AddUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : ICommand<UserDetails>;

    internal class Handler : ICommandHandler<Command, UserDetails>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public Handler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDetails> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                throw new AppException("Passwords do not match.");
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new NotFoundException("User with this email already exists.");
            }

            var user = new User
            {
                Email = request.Email,
                IsActive = true,
            };

            user = await _userRepository.AddUserAsync(user, request.Password);
            return _mapper.Map<UserDetails>(user);
        }
    }
}
