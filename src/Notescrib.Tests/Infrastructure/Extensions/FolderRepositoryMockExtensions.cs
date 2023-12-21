using Moq;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;

namespace Notescrib.Notes.Tests.Infrastructure.Extensions;

public static class FolderRepositoryMockExtensions
{
    public static Mock<IFolderRepository> SetupGetRoot(this Mock<IFolderRepository> mock, Func<Folder?> valueFunc)
    {
        mock.Setup(
                x => x.GetRootAsync(
                    It.IsAny<FolderIncludeOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueFunc.Invoke());

        return mock;
    }

    public static Mock<IFolderRepository> SetupGetById(this Mock<IFolderRepository> mock,
        Func<string, Folder?> valueFunc)
    {
        mock.Setup(
                x => x.GetByIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<FolderIncludeOptions>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((string s, FolderIncludeOptions _, CancellationToken _) => valueFunc.Invoke(s));

        return mock;
    }

    public static Mock<IFolderRepository> SetupCreate(this Mock<IFolderRepository> mock, Action<Folder> callback)
    {
        mock.Setup(
                x => x.CreateAsync(
                    It.IsAny<Folder>(),
                    It.IsAny<CancellationToken>()))
            .Callback((Folder f, CancellationToken _) => callback.Invoke(f));

        return mock;
    }

    public static Mock<IFolderRepository> SetupDeleteMany(this Mock<IFolderRepository> mock,
        Action<IEnumerable<string>> callback)
    {
        mock.Setup(
                x => x.DeleteManyAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<string> ids, CancellationToken _) => callback.Invoke(ids));

        return mock;
    }
}
