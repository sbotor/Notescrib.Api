using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Mappers;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Notes.Queries.GetNotes;

namespace Notescrib.Notes.Tests.Features.Notes.Queries;

public class GetNotesQueryHandlerTests
{
    private static int Counter;
    
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestNoteRepository _repository = new();
    
    private readonly Handler _sut;

    public GetNotesQueryHandlerTests()
    {
        _sut = new(_repository, new PermissionGuard(_userContext), new NoteOverviewMapper(), new NotesSortingProvider());
        
        _repository.Items.AddRange(new[]
        {
            CreateNote("1", string.Empty, "1"),
            CreateNote("1", string.Empty, "2"),
            CreateNote("2", string.Empty, "1"),
            CreateNote("2", string.Empty, "2"),
            CreateNote("3", string.Empty, "1"),
            CreateNote("1", "Folder 1", "1"),
            CreateNote("1", "Folder 1", "2")
        });

        _userContext.UserId = "1";
    }

    [Fact]
    public async Task Handle_ForPrivateNotes_ReturnsCorrectNotes()
    {
        var result = await _sut.Handle(
            new(
                null,
                new(1, 10),
                new()),
            default);
        
        Assert.Equal(4, result.Data.Count);
        Assert.All(result.Data, x => Assert.Equal("1", x.OwnerId));
    }
    
    // TODO: More tests here.

    private static Note CreateNote(string workspaceId, string folder, string ownerId, SharingInfo? sharingInfo = null)
        => new()
        {
            Name = $"Note {Counter}",
            FolderId = folder,
            Id = Counter++.ToString(),
            OwnerId = ownerId,
            SharingInfo = sharingInfo ?? new()
        };
}
