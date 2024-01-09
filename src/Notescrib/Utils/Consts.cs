namespace Notescrib.Utils;

public static class Consts
{
    public static class Name
    {
        public const int MaxLength = 100;
    }

    public static class Note
    {
        public const int MaxContentLength = 1_000_000;
        public const int MaxTagCount = 10;
        public const int MaxTagLength = 50;
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

    public static class OwnerId
    {
        public const int MaxLength = 40;
    }
}
