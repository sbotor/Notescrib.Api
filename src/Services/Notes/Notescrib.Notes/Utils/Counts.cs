namespace Notescrib.Notes.Utils;

public static class Counts
{
    public static class Name
    {
        public const int MaxLength = 50;
    }

    public static class Note
    {
        public const int MaxLength = 10_000;
        public const int MaxLabelCount = 10;
        public const int MaxSharingCount = 10;
        public const int MaxNestingLevel = 2;
    }

    public static class Folder
    {
        public const int MaxNestingLevel = 4;
    }

    public static class Workspace
    {
        public const int MaxCount = 10;
    }
}
