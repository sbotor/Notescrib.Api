using MediatR;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Users.Mappers;
using Notescrib.Identity.Features.Users.Models;

namespace Notescrib.Identity.Features.Users.Queries;

public static class GetUserDetails
{
    public record Query(string Id) : IRequest<UserDetails>;

    internal class Handler : IRequestHandler<Query, UserDetails>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserMapper _mapper;

        public Handler(AppUserManager userManager, IUserMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var found = await _userManager.FindByIdAsync(request.Id);
            if (found == null)
            {
                throw new NotFoundException<AppUser>();
            }

            return _mapper.MapToDetails(found);
        }
    }
}
