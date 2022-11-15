using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Notes.Utils;

public class NoteSectionTree : BfsTree<NoteSection>
{
    public NoteSectionTree(IEnumerable<NoteSection> roots) : base(roots)
    {
    }

    public NoteSectionTree()
    {
    }

    public void ValidateAndThrow()
    {
        var nameSet = new HashSet<string>();
        var charCount = 0;
        
        foreach (var node in AsNodeEnumerable())
        {
            if (node.Level >= Size.NestingLevel.Max)
            {
                throw new AppException();
            }

            if (nameSet.Contains(node.Item.Name))
            {
                throw new DuplicationException<NoteSection>();
            }
            nameSet.Add(node.Item.Name);
            
            charCount += node.Item.Content.Length;
            if (charCount > Size.Counts.MaxNoteLength)
            {
                throw new AppException("The note is too long.");
            }
        }
    }
}
