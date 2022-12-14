namespace Notescrib.Notes.Utils;

public static class Consts
{
    public static class Name
    {
        public const int MaxLength = 50;
    }

    public static class Note
    {
        public const int MaxContentLength = 1_000_000;
        public const int MaxLabelCount = 10;
        public const int MaxRelatedCount = 10;
    }

    public static class Folder
    {
        public const int MaxNestingLevel = 8;
        public const int MaxChildrenCount = 10;
        public const int MaxNoteCount = 32;
    }

    public static class NoteTemplate
    {
        public const int MaxContentLength = 100_000;
    }
}
