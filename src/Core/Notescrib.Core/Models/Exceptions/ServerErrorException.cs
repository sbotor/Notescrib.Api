namespace Notescrib.Core.Models.Exceptions;

public class ServerErrorException : AppException
{
    public ServerErrorException(string code, string? message = null) : base(code, message)
    {
    }    
}
