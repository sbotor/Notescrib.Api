namespace Notescrib.Notes.Application.Common.Providers;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime Today { get; }
    DateTime UtcNow { get; }
}