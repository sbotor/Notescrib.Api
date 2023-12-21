using Moq;
using Notescrib.Core.Services;

namespace Notescrib.Tests.Infrastructure.Extensions;

public static class UserContextProviderExtensions
{
    public static void SetupUserId(this Mock<IUserContextProvider> mock, string? userId)
    {
        if (userId == null)
        {
            mock.Setup(x => x.UserId)
                .Throws(() => new InvalidOperationException("No UserId"));
        }
        else
        {
            mock.Setup(x => x.UserId)
                .Returns(userId); 
        }

        mock.Setup(x => x.IsAnonymous)
            .Returns(() => userId == null);

        mock.Setup(x => x.UserIdOrDefault)
            .Returns(userId);
    }
}
