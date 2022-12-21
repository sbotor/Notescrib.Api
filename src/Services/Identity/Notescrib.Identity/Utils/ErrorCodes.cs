namespace Notescrib.Identity.Utils;

public class ErrorCodes
{
    public static class User
    {
        public const string UserNotFound = nameof(UserNotFound);
        public const string EmailNotConfirmed = nameof(EmailNotConfirmed);
        public const string EmailAlreadyConfirmed = nameof(EmailAlreadyConfirmed);
        public const string UserInactive = nameof(UserInactive);
        public const string PasswordsDoNotMatch = nameof(PasswordsDoNotMatch);
        public const string EmailTaken = nameof(EmailTaken);
    }

    public static class Notes
    {
        public const string NotesError = nameof(NotesError);
    }
    
    public static class Emails
    {
        public const string EmailsError = nameof(EmailsError);
    }
}
