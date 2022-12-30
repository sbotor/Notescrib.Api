using Moq;
using Notescrib.Notes.Features.Notes.Repositories;

namespace Notescrib.Notes.Tests.Infrastructure.Extensions;

public static class NoteRepositoryMockExtensions
{
    public static Mock<INoteRepository> SetupGetIdsFromFolders(this Mock<INoteRepository> mock,
        Func<IEnumerable<string>, IReadOnlyCollection<string>> valueFunc)
    {
        mock.Setup(
                x => x.GetIdsFromFoldersAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<string> folderIds, CancellationToken _) => valueFunc.Invoke(folderIds));

        return mock;
    }
    
    public static Mock<INoteRepository> SetupDeleteFromRelated(this Mock<INoteRepository> mock,
        Action<IEnumerable<string>> callback)
    {
        mock.Setup(
                x => x.DeleteFromRelatedAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<string> x, CancellationToken _) => callback.Invoke(x));

        return mock;
    }
    
    public static Mock<INoteRepository> SetupDeleteMany(this Mock<INoteRepository> mock,
        Action<IEnumerable<string>> callback)
    {
        mock.Setup(
                x => x.DeleteManyAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<string> x, CancellationToken _) => callback.Invoke(x));

        return mock;
    }
}
