using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class AppException : Exception
{
    public IReadOnlyCollection<ErrorItem> ErrorData { get; }

    public AppException(string? message = null, IEnumerable<ErrorItem>? errorData = null)
        : base(message ?? ErrorModel.DefaultMessage)
    {
        ErrorData = errorData?.ToArray() ?? Array.Empty<ErrorItem>();
    }

    public AppException(IEnumerable<ErrorItem> errorData)
        : this(ErrorModel.DefaultMessage, errorData)
    {
    }

    public ErrorModel ToErrorModel()
        => new(Message, ErrorData);
}
