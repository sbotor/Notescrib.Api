namespace Notescrib.Core.Models.Exceptions;

public class DuplicationException : AppException
{
    public DuplicationException(string? message = null)
    {
    }
}

public class DuplicationException<TEntity> : DuplicationException
{
    public DuplicationException()
        : base($"'{typeof(TEntity).Name}' already exists.")
    {
    }
}
