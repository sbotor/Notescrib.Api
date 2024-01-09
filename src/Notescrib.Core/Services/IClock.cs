namespace Notescrib.Core.Services;

public interface IClock
{
    DateTime Now { get; }
}

public class UtcClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}
