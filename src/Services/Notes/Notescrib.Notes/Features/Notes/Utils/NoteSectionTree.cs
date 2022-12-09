using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Notes.Utils;

public class NoteSectionTree : BfsTree<NoteSection>
{
    public NoteSectionTree(IEnumerable<NoteSection> roots) : base(NoteSection.CreateRoot(roots))
    {
    }

    public void ValidateAndThrow()
    {
        var nameSet = new HashSet<string>();
        var charCount = 0;
        
        foreach (var node in AsNodeEnumerable())
        {
            if (node.Level >= Counts.NestingLevel.Max)
            {
                throw new AppException();
            }

            if (nameSet.Contains(node.Item.Name))
            {
                throw new DuplicationException<NoteSection>();
            }
            nameSet.Add(node.Item.Name);
            
            charCount += node.Item.Content.Length;
            if (charCount > Counts.Note.MaxLength)
            {
                throw new AppException("The note is too long.");
            }
        }
    }
}
