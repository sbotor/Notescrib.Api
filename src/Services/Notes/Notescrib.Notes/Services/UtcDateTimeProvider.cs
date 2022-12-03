namespace Notescrib.Notes.Services;

internal class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
