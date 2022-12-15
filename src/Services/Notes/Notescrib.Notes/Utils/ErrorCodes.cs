namespace Notescrib.Notes.Utils;

public static class ErrorCodes
{
    public static class Workspace
    {
        public const string WorkspaceNotFound = nameof(WorkspaceNotFound);
        public const string WorkspaceAlreadyExists = nameof(WorkspaceAlreadyExists);
    }
    
    public static class Folder
    {
        public const string MaximumFolderCountReached = nameof(MaximumFolderCountReached);
        public const string MaximumNoteCountReached = nameof(MaximumFolderCountReached);
        public const string CannotNestMoreChildren = nameof(CannotNestMoreChildren);
        public const string FolderAlreadyExists = nameof(FolderAlreadyExists);
        public const string FolderNotFound = nameof(FolderNotFound);
    }

    public static class Note
    {
        public const string NoteNotFound = nameof(NoteNotFound);
        public const string NoteAlreadyExists = nameof(NoteAlreadyExists);
    }
}
