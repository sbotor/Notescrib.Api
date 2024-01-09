using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Users.Mappers;
using Notescrib.Identity.Features.Users.Models;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Queries;

public static class GetUserDetails
{
    public record Query() : IQuery<UserDetails>;

    internal class Handler : IQueryHandler<Query, UserDetails>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserMapper _mapper;
        private readonly IUserContext _userContext;

        public Handler(AppUserManager userManager, IUserMapper mapper, IUserContext userContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<UserDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var userInfo = await _userContext.GetUserInfo(CancellationToken.None);
            
            var found = await _userManager.FindByIdAsync(userInfo.UserId);
            if (found == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }

            return _mapper.MapToDetails(found);
        }
    }
}
