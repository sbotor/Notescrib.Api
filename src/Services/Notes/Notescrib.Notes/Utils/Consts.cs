namespace Notescrib.Notes.Utils;

public static class Consts
{
    public static class Name
    {
        public const int MaxLength = 50;
    }

    public static class Note
    {
        public const int MaxTotalLength = 100_000;
        public const int MaxSectionLength = 10_000;
        public const int MaxSectionCount = 10;
        public const int MaxLabelCount = 10;
        public const int MaxSharingCount = 10;
        public const int MaxNestingLevel = 2;
    }

    public static class Folder
    {
        public const int MaxNestingLevel = 4;
        public const int MaxChildrenCount = 10;
    }

    public static class Workspace
    {
        public const int MaxNoteCount = 128;
    }
}
